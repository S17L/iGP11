#define SCREEN_WIDTH 1920
#define SCREEN_HEIGHT 1080

#define PIXEL_WIDTH 1.0 / SCREEN_WIDTH
#define PIXEL_HEIGHT 1.0 / SCREEN_HEIGHT
static const float2 _texel = float2(PIXEL_WIDTH, PIXEL_HEIGHT);

#define LUMINESCENCE_COEFFICIENT_TYPE 0
#if LUMINESCENCE_COEFFICIENT_TYPE == 0
static const float3 _luminescence_coefficient = float3(0.2126, 0.7152, 0.0722);
#else
static const float3 _luminescence_coefficient = float3(0.299, 0.587, 0.114);
#endif

Texture2D _color_texture : register(t0);
Texture2D _depth_texture : register(t1);

SamplerState _point_sampler : register(s0);
SamplerState _bilinear_sampler : register(s1);

struct PixelInputType
{
    float4 position : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};

/* BEGIN -> COMMON FUNCTIONS */

float getLuminescence(float3 color)
{
    return saturate(dot(color, _luminescence_coefficient));
}

float3 setLuminescence(float3 color, float luminescence)
{
    return color * (saturate(luminescence) / getLuminescence(color));
}

float scale(float value, float oldMin, float oldMax, float newMin, float newMax)
{
    return ((newMax - newMin) / (oldMax - oldMin)) * (value - oldMax) + newMax;
}

/* END -> COMMON FUNCTIONS */

/* BEGIN -> DEPTH TRANSFORMATION */

#define DEPTH_BUFFER_AVAILABLE 0
#define DEPTH_BUFFER_ACCESS_LINEAR 1

#define DEPTH_LINEAR_Z_NEAR 1.0
#define DEPTH_LINEAR_Z_FAR  1000.0

#define DEPTH_LIMIT_AVAILABLE 0
#define DEPTH_MIN 0.04
#define DEPTH_MAX 0.7

float getLinearDepth(float value)
{
    float normalized = 2.0 * value - 1.0;
    float depth = 2.0 * DEPTH_LINEAR_Z_NEAR * DEPTH_LINEAR_Z_FAR / (DEPTH_LINEAR_Z_FAR + DEPTH_LINEAR_Z_NEAR - normalized * (DEPTH_LINEAR_Z_FAR - DEPTH_LINEAR_Z_NEAR));

    return scale(depth, DEPTH_LINEAR_Z_NEAR, DEPTH_LINEAR_Z_FAR, 0, 1);
}

float getDepth(float2 texcoord)
{
    float depth = 0;
#if defined(DEPTH_BUFFER_AVAILABLE)
    depth = _depth_texture.Sample(_bilinear_sampler, texcoord).x;
#if defined(DEPTH_BUFFER_ACCESS_LINEAR)
    depth = getLinearDepth(depth);
#endif
#endif
#if DEPTH_LIMIT_AVAILABLE == 0
    return depth;
#else
    return scale(max(DEPTH_MIN, min(DEPTH_MAX, depth)), DEPTH_MIN, DEPTH_MAX, 0, 1);
#endif
}

/* END -> DEPTH TRANSFORMATION */

/* BEGIN -> BOKEH DOF */

#define BOKEH_DOF_ENABLED 0
#define BOKEH_DOF_PRESERVE_SHAPE 0
#define BOKEH_DOF_SHAPE_PASS_0_TEXEL_COUNT 0
#define BOKEH_DOF_SHAPE_PASS_1_TEXEL_COUNT 0
#define BOKEH_DOF_SHAPE_PASS_2_TEXEL_COUNT 0
#define BOKEH_DOF_BLUR_SIZE 2
#define BOKEH_DOF_BLUR_STRENGTH 1.0
#define BOKEH_DOF_DEPTH_MIN 0.0
#define BOKEH_DOF_DEPTH_MAX 1.0
#define BOKEH_DOF_DEPTH_RATE_GAIN 2.0
#define BOKEH_DOF_LUMINESCENCE_MIN 0.1
#define BOKEH_DOF_LUMINESCENCE_MAX 1.0
#define BOKEH_DOF_LUMINESCENCE_REAL_MIN min(BOKEH_DOF_LUMINESCENCE_MIN, BOKEH_DOF_LUMINESCENCE_MAX)
#define BOKEH_DOF_LUMINESCENCE_REAL_MAX max(BOKEH_DOF_LUMINESCENCE_MIN, BOKEH_DOF_LUMINESCENCE_MAX)
#define BOKEH_DOF_LUMINESCENCE_RATE_GAIN 2.0
#define BOKEH_DOF_CHROMATIC_ABERRATION_ENABLED 0
#define BOKEH_DOF_CHROMATIC_ABERRATION_FRINGE 0.5
#define BOKEH_DOF_BLUR_STRENGTH_OFFSET 0.1
#define BOKEH_DOF_BLUR_STRENGTH_MIN pow(BOKEH_DOF_DEPTH_MIN, BOKEH_DOF_DEPTH_RATE_GAIN) * pow(BOKEH_DOF_LUMINESCENCE_REAL_MIN, BOKEH_DOF_LUMINESCENCE_RATE_GAIN) + BOKEH_DOF_BLUR_STRENGTH_OFFSET
#define BOKEH_DOF_BLUR_STRENGTH_MAX pow(BOKEH_DOF_DEPTH_MAX, BOKEH_DOF_DEPTH_RATE_GAIN) * pow(BOKEH_DOF_LUMINESCENCE_REAL_MAX, BOKEH_DOF_LUMINESCENCE_RATE_GAIN) + BOKEH_DOF_BLUR_STRENGTH_OFFSET

#if BOKEH_DOF_ENABLED == 1
Texture2D<float4> _tex_0 : register(t2);
Texture2D<float4> _tex_1 : register(t3);

static const float _bokeh_dof_blur_offset[BOKEH_DOF_BLUR_SIZE] = { 0.000000, 1.002473 };
static const float _bokeh_dof_blur_weight[BOKEH_DOF_BLUR_SIZE] = { 0.786986, 0.106771 };

float calculateCoC(float3 color, float depth)
{
    float luminescence = getLuminescence(color);

    return pow(lerp(BOKEH_DOF_DEPTH_MIN, BOKEH_DOF_DEPTH_MAX, depth), BOKEH_DOF_DEPTH_RATE_GAIN)
        * pow(lerp(BOKEH_DOF_LUMINESCENCE_MIN, BOKEH_DOF_LUMINESCENCE_MAX, luminescence), BOKEH_DOF_LUMINESCENCE_RATE_GAIN)
        + BOKEH_DOF_BLUR_STRENGTH_OFFSET;
}

float getCoC(float2 texcoord)
{
    return calculateCoC(_color_texture.Sample(_bilinear_sampler, texcoord).xyz, getDepth(texcoord));
}

float4 limitColor(float4 color, float coc, int texelCount)
{
#if BOKEH_DOF_PRESERVE_SHAPE == 0
    color /= texelCount;
#else
    color /= color.w;
    color = saturate(color);
    color *= coc;
    color.w = coc;
#endif

    return color;
}

float4 psBokehDoFCoC(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_point_sampler, input.texcoord);
    float depth = getDepth(input.texcoord);
    float coc = calculateCoC(color.xyz, depth);

    float outsideCoC = getCoC(input.texcoord + float2(-0.5, 1.5) * _texel)
        + getCoC(input.texcoord + float2(1.5, 0.5) * _texel)
        + getCoC(input.texcoord + float2(0.5, -1.5) * _texel)
        + getCoC(input.texcoord + float2(-1.5, -0.5) * _texel);

    outsideCoC /= 4;
    coc = min(coc, outsideCoC);

    color *= coc;
    color.w = coc;

    return color;
}

float4 psBokehDoFShapePass_0(PixelInputType input) : SV_TARGET
{
    float4 bokehDoFColor = _tex_0.Sample(_point_sampler, input.texcoord);
    float bokehDoFCoC = bokehDoFColor.w;

    /* BOKEH_DOF_SHAPE_PASS_0_PLACEHOLDER */

    return limitColor(bokehDoFColor, bokehDoFCoC, BOKEH_DOF_SHAPE_PASS_0_TEXEL_COUNT);
}

float4 psBokehDoFShapePass_1(PixelInputType input) : SV_TARGET
{
    float4 bokehDoFColor = _tex_1.Sample(_point_sampler, input.texcoord);
    float bokehDoFCoC = bokehDoFColor.w;

    /* BOKEH_DOF_SHAPE_PASS_1_PLACEHOLDER */

    return limitColor(bokehDoFColor, bokehDoFCoC, BOKEH_DOF_SHAPE_PASS_1_TEXEL_COUNT);
}

float4 psBokehDoFShapePass_2(PixelInputType input) : SV_TARGET
{
    float4 bokehDoFColor = _tex_0.Sample(_point_sampler, input.texcoord);
    float bokehDoFCoC = bokehDoFColor.w;

    /* BOKEH_DOF_SHAPE_PASS_2_PLACEHOLDER */

    return limitColor(bokehDoFColor, bokehDoFCoC, BOKEH_DOF_SHAPE_PASS_2_TEXEL_COUNT);
}

float4 psBokehDoFHorizontalGaussianBlur(PixelInputType input) : SV_TARGET
{
    float4 color = _tex_1.Sample(_point_sampler, input.texcoord) * _bokeh_dof_blur_weight[0];

    [unroll(BOKEH_DOF_BLUR_SIZE - 1)]
    for (uint i = 1; i < BOKEH_DOF_BLUR_SIZE; i++)
    {
        color += _tex_1.Sample(_bilinear_sampler, input.texcoord + float2(-_bokeh_dof_blur_offset[i], 0) * _texel) * _bokeh_dof_blur_weight[i];
        color += _tex_1.Sample(_bilinear_sampler, input.texcoord + float2(_bokeh_dof_blur_offset[i], 0) * _texel) * _bokeh_dof_blur_weight[i];
    }

    return color;
}

float4 psBokehDoFVerticalGaussianBlur(PixelInputType input) : SV_TARGET
{
    float4 color = _tex_0.Sample(_point_sampler, input.texcoord) * _bokeh_dof_blur_weight[0];

    [unroll(BOKEH_DOF_BLUR_SIZE - 1)]
    for (uint i = 1; i < BOKEH_DOF_BLUR_SIZE; i++)
    {
        color += _tex_0.Sample(_bilinear_sampler, input.texcoord + float2(0, -_bokeh_dof_blur_offset[i]) * _texel) * _bokeh_dof_blur_weight[i];
        color += _tex_0.Sample(_bilinear_sampler, input.texcoord + float2(0, _bokeh_dof_blur_offset[i]) * _texel) * _bokeh_dof_blur_weight[i];
    }

    return color;
}

float4 psBokehDoFChromaticAberration(PixelInputType input) : SV_TARGET
{
    float4 color = _tex_1.Sample(_point_sampler, input.texcoord);
#if BOKEH_DOF_CHROMATIC_ABERRATION_ENABLED == 1
    float ratio = saturate(color.w - BOKEH_DOF_BLUR_STRENGTH_MIN) / (BOKEH_DOF_BLUR_STRENGTH_MAX - BOKEH_DOF_BLUR_STRENGTH_MIN);
    return float4(
        _tex_1.Sample(_bilinear_sampler, input.texcoord + float2(0, 1) * _texel * ratio * BOKEH_DOF_CHROMATIC_ABERRATION_FRINGE).x,
        _tex_1.Sample(_bilinear_sampler, input.texcoord + float2(-0.866, -0.5) * _texel * ratio * BOKEH_DOF_CHROMATIC_ABERRATION_FRINGE).y,
        _tex_1.Sample(_bilinear_sampler, input.texcoord + float2(0.866, -0.5) * _texel * ratio * BOKEH_DOF_CHROMATIC_ABERRATION_FRINGE).z,
        color.w);
#else
    return color;
#endif
}

float4 psBokehDoFBlend(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_point_sampler, input.texcoord);
    float4 bokehDoFColor = _tex_0.Sample(_point_sampler, input.texcoord);
    float weight = BOKEH_DOF_BLUR_STRENGTH * getDepth(input.texcoord);

    color.rgb = (color.rgb + weight * bokehDoFColor.rgb) / (1 + weight * bokehDoFColor.w);
    color.rgb = saturate(color.rgb);

    return color;
}
#endif

/* END -> BOKEH DOF */

/* BEGIN -> DENOISE */

#define DENOISE_ENABLED 0
#define DENOISE_NOISE_LEVEL 0.15
#define DENOISE_BLENDING_COEFFICIENT 0.8
#define DENOISE_WEIGHT_THRESHOLD 0.03
#define DENOISE_COUNTER_THRESHOLD 0.05
#define DENOISE_GAUSSIAN_SIGMA 50.0
#define DENOISE_WINDOW_SIZE 3
#define DENOISE_WINDOW_AREA pow(2.0 * DENOISE_WINDOW_SIZE + 1.0, 2)

#if DENOISE_ENABLED == 1
float4 psDenoise(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_point_sampler, input.texcoord);
    float4 output = 0;
    float counter = 0;
    float sum = 0;

    for (int i = -DENOISE_WINDOW_SIZE; i <= DENOISE_WINDOW_SIZE; i++)
    {
        for (int j = -DENOISE_WINDOW_SIZE; j <= DENOISE_WINDOW_SIZE; j++)
        {
            float4 currentColor = _color_texture.Sample(_point_sampler, input.texcoord + float2(i, j) * _texel);
            float weight = dot(color - currentColor, color - currentColor);

            weight = exp(-(weight * rcp(DENOISE_NOISE_LEVEL) + (i * i + j * j) * rcp(DENOISE_GAUSSIAN_SIGMA)));
            counter += weight > DENOISE_WEIGHT_THRESHOLD;
            sum += weight;
            output += currentColor * weight;
        }
    }

    output /= sum;
    float coefficient = (counter > (DENOISE_COUNTER_THRESHOLD * DENOISE_WINDOW_AREA)) ? 1.0 - DENOISE_BLENDING_COEFFICIENT : DENOISE_BLENDING_COEFFICIENT;
    output = lerp(output, color, coefficient);

    return output;
}
#endif

/* END -> DENOISE */

/* BEGIN -> GAUSSIAN BLUR */

#define GAUSSIAN_BLUR_ENABLED 0
#define GAUSSIAN_BLUR_SIZE 2

#if GAUSSIAN_BLUR_ENABLED == 1
static const float _gaussianblur_offset[GAUSSIAN_BLUR_SIZE] = { 0.000000, 1.002473 };
static const float _gaussianblur_weight[GAUSSIAN_BLUR_SIZE] = { 0.786986, 0.106771 };

float4 psHorizontalGaussianBlur(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_bilinear_sampler, input.texcoord) * _gaussianblur_weight[0];

    [unroll(GAUSSIAN_BLUR_SIZE - 1)]
    for (uint i = 1; i < GAUSSIAN_BLUR_SIZE; i++)
    {
        color += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-_gaussianblur_offset[i], 0) * _texel) * _gaussianblur_weight[i];
        color += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(_gaussianblur_offset[i], 0) * _texel) * _gaussianblur_weight[i];
    }

    return color;
}

float4 psVerticalGaussianBlur(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_bilinear_sampler, input.texcoord) * _gaussianblur_weight[0];

    [unroll(GAUSSIAN_BLUR_SIZE - 1)]
    for (uint i = 1; i < GAUSSIAN_BLUR_SIZE; i++)
    {
        color += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(0, -_gaussianblur_offset[i]) * _texel) * _gaussianblur_weight[i];
        color += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(0, _gaussianblur_offset[i]) * _texel) * _gaussianblur_weight[i];
    }

    return color;
}
#endif

/* END -> GAUSSIAN BLUR */

/* BEGIN -> HDR */

#define HDR_ENABLED 0
#define HDR_STRENGTH 1.3
#define HDR_RADIUS_1 0.793
#define HDR_RADIUS_2 0.87

#if HDR_ENABLED == 1
float4 psHDR(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_point_sampler, input.texcoord);

    float3 bloomSum1 = _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(1.5, -1.5) * HDR_RADIUS_1).rgb;
    bloomSum1 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-1.5, -1.5) * HDR_RADIUS_1).rgb;
    bloomSum1 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(1.5, 1.5) * HDR_RADIUS_1).rgb;
    bloomSum1 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-1.5, 1.5) * HDR_RADIUS_1).rgb;
    bloomSum1 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(0.0, -2.5) * HDR_RADIUS_1).rgb;
    bloomSum1 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(0.0, 2.5) * HDR_RADIUS_1).rgb;
    bloomSum1 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-2.5, 0.0) * HDR_RADIUS_1).rgb;
    bloomSum1 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(2.5, 0.0) * HDR_RADIUS_1).rgb;
    bloomSum1 *= 0.005;

    float3 bloomSum2 = _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(1.5, -1.5) * HDR_RADIUS_2).rgb;
    bloomSum2 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-1.5, -1.5) * HDR_RADIUS_2).rgb;
    bloomSum2 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(1.5, 1.5) * HDR_RADIUS_2).rgb;
    bloomSum2 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-1.5, 1.5) * HDR_RADIUS_2).rgb;
    bloomSum2 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(0.0, -2.5) * HDR_RADIUS_2).rgb;
    bloomSum2 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(0.0, 2.5) * HDR_RADIUS_2).rgb;
    bloomSum2 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-2.5, 0.0) * HDR_RADIUS_2).rgb;
    bloomSum2 += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(2.5, 0.0) * HDR_RADIUS_2).rgb;
    bloomSum2 *= 0.010;

    float distance = HDR_RADIUS_2 - HDR_RADIUS_1;
    float3 HDR = (color.rgb + (bloomSum2 - bloomSum1)) * distance;
    float3 weight = HDR + color.rgb;
    color.rgb = pow(abs(weight), abs(HDR_STRENGTH)) + HDR;
    color.rgb = saturate(color.rgb);
	
    return color;
}
#endif

/* END -> HDR */

/* BEGIN -> LIFTGAMMAGAIN */

#define LIFTGAMMAGAIN_ENABLED 0
#define LIFTGAMMAGAIN_LIFT_RED 1.0
#define LIFTGAMMAGAIN_LIFT_GREEN 1.0
#define LIFTGAMMAGAIN_LIFT_BLUE 1.0
#define LIFTGAMMAGAIN_GAMMA_RED 1.0
#define LIFTGAMMAGAIN_GAMMA_GREEN 1.0
#define LIFTGAMMAGAIN_GAMMA_BLUE 1.0
#define LIFTGAMMAGAIN_GAIN_RED 1.0
#define LIFTGAMMAGAIN_GAIN_GREEN 1.0
#define LIFTGAMMAGAIN_GAIN_BLUE 1.0

#if LIFTGAMMAGAIN_ENABLED == 1
static const float3 _lift_color = float3(LIFTGAMMAGAIN_LIFT_RED, LIFTGAMMAGAIN_LIFT_GREEN, LIFTGAMMAGAIN_LIFT_BLUE);
static const float3 _gamma_color = float3(LIFTGAMMAGAIN_GAMMA_RED, LIFTGAMMAGAIN_GAMMA_GREEN, LIFTGAMMAGAIN_GAMMA_BLUE);
static const float3 _gain_color = float3(LIFTGAMMAGAIN_GAIN_RED, LIFTGAMMAGAIN_GAIN_GREEN, LIFTGAMMAGAIN_GAIN_BLUE);

float4 psLiftGammaGain(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_point_sampler, input.texcoord);
    
    color.rgb = color.rgb * (1.5 - 0.5 * _lift_color) + 0.5 * _lift_color - 0.5;
	color.rgb = saturate(color.rgb);
    color.rgb *= _gain_color;
    color.rgb = pow(color.rgb, 1.0 / _gamma_color);
    color.rgb = saturate(color.rgb);

    return color;
}
#endif

/* END -> LIFTGAMMAGAIN */

/* BEGIN -> LUMASHARPEN */

#define LUMASHARPEN_ENABLED 0
#define LUMASHARPEN_SHARPENING_STRENGTH 1.25
#define LUMASHARPEN_SHARPENING_CLAMP 0.035
#define LUMASHARPEN_OFFSET 1

#if LUMASHARPEN_ENABLED == 1
float4 psLumaSharpen(PixelInputType input) : SV_TARGET
{
    float3 color = _color_texture.Sample(_point_sampler, input.texcoord).rgb;
    float3 sharpeningStrengthLuma = LUMASHARPEN_SHARPENING_STRENGTH * _luminescence_coefficient;

    float3 blurColor = _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(1, -1) * _texel * 0.5 * LUMASHARPEN_OFFSET).rgb;
    blurColor += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-1, -1) * _texel * 0.5 * LUMASHARPEN_OFFSET).rgb;
    blurColor += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(1, 1) * _texel * 0.5 * LUMASHARPEN_OFFSET).rgb;
    blurColor += _color_texture.Sample(_bilinear_sampler, input.texcoord + float2(-1, 1) * _texel * 0.5 * LUMASHARPEN_OFFSET).rgb;
    blurColor /= 4;

    float3 sharp = color - blurColor;
    float4 sharpeningStrengthLumaClamp = float4(sharpeningStrengthLuma * (0.5 / LUMASHARPEN_SHARPENING_CLAMP), 0.5);

    float sharpeningLuma = saturate(dot(float4(sharp,1.0), sharpeningStrengthLumaClamp));
    sharpeningLuma = (LUMASHARPEN_SHARPENING_CLAMP * 2.0) * sharpeningLuma - LUMASHARPEN_SHARPENING_CLAMP;

    color = color + sharpeningLuma;

    return float4(color.r, color.g, color.b, 0);
}
#endif

/* END -> LUMASHARPEN */

/* BEGIN -> TONEMAP */

#define TONEMAP_ENABLED 0
#define TONEMAP_GAMMA 1
#define TONEMAP_EXPOSURE 0
#define TONEMAP_SATURATION 0
#define TONEMAP_BLEACH 0
#define TONEMAP_DEFOG 0
#define TONEMAP_FOG_RED 0
#define TONEMAP_FOG_GREEN 0
#define TONEMAP_FOG_BLUE 1.0

#if TONEMAP_ENABLED == 1
static const float3 _defog_color = TONEMAP_DEFOG * float3(TONEMAP_FOG_RED, TONEMAP_FOG_GREEN, TONEMAP_FOG_BLUE);

float4 psTonemap(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_point_sampler, input.texcoord);
    color.rgb = saturate(color.rgb - _defog_color * 2.55);
    color.rgb *= pow(2.0f, TONEMAP_EXPOSURE);
    color.rgb = pow(color.rgb, TONEMAP_GAMMA);
    float luminescence = getLuminescence(color.rgb);
    float coefficient = TONEMAP_BLEACH * color.rgb;
    color.rgb += ((1.0f - coefficient) * coefficient * lerp(2.0f * color.rgb * luminescence.rrr, 1.0f - 2.0f * (1.0f - luminescence.rrr) * (1.0f - color.rgb), saturate(10.0 * (luminescence - 0.45))));
    float3 tonemapColor = color.rgb - dot(color.rgb, (1.0 / 3.0));
    color.rgb = (color.rgb + TONEMAP_SATURATION * tonemapColor) / (1 + (TONEMAP_SATURATION * tonemapColor));

    return color;
}
#endif

/* END -> TONEMAP */

/* BEGIN -> VIBRANCE */

#define VIBRANCE_ENABLED 0
#define VIBRANCE_STRENGTH 0.3
#define VIBRANCE_GAIN_RED 1.0
#define VIBRANCE_GAIN_GREEN 1.0
#define VIBRANCE_GAIN_BLUE 1.0

#if VIBRANCE_ENABLED == 1
static const float3 _vibrance_coefficient = VIBRANCE_STRENGTH * float3(VIBRANCE_GAIN_RED, VIBRANCE_GAIN_GREEN, VIBRANCE_GAIN_BLUE);

float4 psVibrance(PixelInputType input) : SV_TARGET
{
    float4 color = _color_texture.Sample(_point_sampler, input.texcoord);
    float luminescence = getLuminescence(color.rgb);
    float maxColor = max(color.r, max(color.g, color.b));
    float minColor = min(color.r, min(color.g, color.b));
    float saturation = maxColor - minColor;
    color.rgb = lerp(luminescence, color.rgb, (1 + (_vibrance_coefficient * (1 - (sign(_vibrance_coefficient) * saturation)))));

    return color;
}
#endif

/* END -> VIBRANCE */

float4 psAlpha(PixelInputType input) : SV_TARGET
{
    float alpha = _color_texture.Sample(_point_sampler, input.texcoord).a;
    return float4(alpha, alpha, alpha, 0);
}

float4 psDepth(PixelInputType input) : SV_TARGET
{
    float depth = getDepth(input.texcoord);
    return float4(depth, depth, depth, 0);
}

float4 psLuminescence(PixelInputType input) : SV_TARGET
{
    float luminescence = getLuminescence(_color_texture.Sample(_point_sampler, input.texcoord).rgb);
    return float4(luminescence, luminescence, luminescence, 0);
}