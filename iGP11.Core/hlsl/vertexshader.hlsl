cbuffer MatrixBuffer
{
    matrix worldMatrix;
    matrix viewMatrix;
    matrix projectionMatrix;
};

struct VertexInputType
{
    float4 position : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct PixelInputType
{
    float4 position : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};

PixelInputType vsmain(VertexInputType input)
{
    PixelInputType output;
    
    input.position.w = 1.0f;
    output.position = mul(input.position, worldMatrix);
    output.position = mul(output.position, viewMatrix);
    output.position = mul(output.position, projectionMatrix);
    output.texcoord = input.texcoord;
    
    return output;
}