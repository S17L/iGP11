#include "stdafx.h"
#include "codebuilder.h"
#include "resourceprovider.h"

using namespace core::logging;

core::dto::GaussianBlur getGaussianBlur(float smoothness) {
    const float coefficient = 5;
    const unsigned int size = 3;

    core::dto::GaussianBlur gaussianBlur;
    gaussianBlur.size = size;
    gaussianBlur.sigma = smoothness * coefficient;

    return gaussianBlur;
}

void calculateGaussianBlur(unsigned int size, float sigma, float minWeight, int &length, float **offsets, float **weights) {
    size *= 2;
    std::unique_ptr<float[]> initial(new float[size + 1]);
    float sum = 0.0;

    for (unsigned int i = 0; i <= size; i++) {
        initial[i] = core::algorithm::gaussian(static_cast<float>(i), sigma);
        if (i > 0 && initial[i] < minWeight) {
            initial[i] = 0;
            size = i;

            break;
        }

        sum += initial[i] * (i == 0 ? 1 : 2);
    }

    for (unsigned int i = 0; i <= size; i++) {
        initial[i] /= sum;
    }

    length = 1 + (size / 2);
    *offsets = new float[length];
    *weights = new float[length];

    for (int i = size, j = length - 1; i > 0; i -= 2, j--) {
        (*weights)[j] = initial[i - 1] + initial[i];
        (*offsets)[j] = ((i - 1) * initial[i - 1] + i * initial[i]) / (*weights)[j];
    }

    (*weights)[0] = initial[0];
    (*offsets)[0] = 0;
}

bool isDistanceSmall(float number) {
    return ::abs(number - ::round(number)) < 0.01;
}

int getSign(float number) {
    return (number > 0) - (number < 0);
}

core::dto::Texel getUpTexel(float x, float y) {
    return core::dto::Texel(static_cast<int>(::round(x)), static_cast<int>(getSign(y) * ::ceil(abs(y))));
}

core::dto::Texel getDownTexel(float x, float y) {
    return core::dto::Texel(static_cast<int>(::round(x)), static_cast<int>(getSign(y) * ::floor(abs(y))));
}

core::dto::Texel getLeftTexel(float x, float y) {
    return core::dto::Texel(static_cast<int>(getSign(x) * ::floor(abs(x))), static_cast<int>(::round(y)));
}

core::dto::Texel getRightTexel(float x, float y) {
    return core::dto::Texel(static_cast<int>(getSign(x) * ::ceil(abs(x))), static_cast<int>(::round(y)));
}

core::dto::Texel getDownLeftTexel(float x, float y) {
    return core::dto::Texel(static_cast<int>(getSign(x) * ::floor(abs(x))), static_cast<int>(getSign(y) * ::floor(abs(y))));
}

core::dto::Texel getUpLeftTexel(float x, float y) {
    return core::dto::Texel(static_cast<int>(getSign(x) * ::floor(abs(x))), static_cast<int>(::isDistanceSmall(abs(y)) ? getSign(y) * ::round(abs(y)) : getSign(y) * ::ceil(abs(y))));
}

core::dto::Texel getDownRightTexel(float x, float y) {
    return core::dto::Texel(static_cast<int>(::isDistanceSmall(abs(x)) ? getSign(x) * ::round(abs(x)) : getSign(x) * ::ceil(abs(x))), static_cast<int>(getSign(y) * ::floor(abs(y))));
}

core::dto::Texel getUpRightTexel(float x, float y) {
    return core::dto::Texel(static_cast<int>(::isDistanceSmall(abs(x)) ? getSign(x) * ::round(abs(x)) : getSign(x) * ::ceil(abs(x))), static_cast<int>(::isDistanceSmall(abs(y)) ? getSign(y) * ::round(abs(y)) : getSign(y) * ::ceil(abs(y))));
}

float getUpTexelArea(float y) {
    return ::abs(y) - ::floor(::abs(y));
}

float getDownTexelArea(float y) {
    return ::ceil(::abs(y)) - ::abs(y);
}

float getLeftTexelArea(float x) {
    return ::ceil(::abs(x)) - ::abs(x);
}

float getRightTexelArea(float x) {
    return ::abs(x) - ::floor(::abs(x));
}

float getDownLeftTexelArea(float x, float y) {
    return (::ceil(::abs(x)) - abs(x)) * (::ceil(abs(y)) - abs(y));
}

float getUpLeftTexelArea(float x, float y) {
    return (::ceil(::abs(x)) - abs(x)) * (abs(y) - ::floor(abs(y)));
}

float getDownRightTexelArea(float x, float y) {
    return (abs(x) - ::floor(abs(x))) * (::ceil(abs(y)) - abs(y));
}

float getUpRightTexelArea(float x, float y) {
    return (abs(x) - ::floor(abs(x))) * (abs(y) - ::floor(abs(y)));
}

std::string translate(bool value) {
    return value ? ENCRYPT_STRING("1") : ENCRYPT_STRING("0");
}

std::string translate(float value) {
    return core::stringFormat(ENCRYPT_STRING("%.5f"), value);
}

std::string translate(int value) {
    return core::stringFormat(ENCRYPT_STRING("%d"), value);
}

std::string translate(unsigned int value) {
    return core::stringFormat(ENCRYPT_STRING("%u"), value);
}

std::string translate(size_t value) {
    return core::stringFormat(ENCRYPT_STRING("%lu"), value);
}

std::list<std::string> translate(float *collection, size_t size) {
    return core::linq::makeEnumerable(collection, size)
        .select<std::string>([](const float &number)->std::string { return translate(number); })
        .toList();
}

std::string core::DefineElement::translate() {
    return core::stringFormat(ENCRYPT_STRING("#define %s %s"), _name.c_str(), _value.c_str());
}

core::CodeVisitor::CodeVisitor(const std::string &code) {
    std::istringstream stream(code);
    std::string line;
    while (std::getline(stream, line)) {
        core::trim(line);
        std::vector<std::string> words = core::split(line);
        if (core::startsWith(line, ENCRYPT_STRING("#define"))) {
            if (words.size() == 3) {
                std::shared_ptr<core::DefineElement> element(new core::DefineElement(words[1], words[2]));
                _defines.push_back(element);
                _elements.push_back(element);

                continue;
            }
        }

        std::shared_ptr<core::LineElement> element(new core::LineElement(line));
        _lines.push_back(element);
        _elements.push_back(element);
    }
}

void core::CodeVisitor::visitDefine(const std::string &name, const std::string &value) {
    for (auto element : _defines) {
        if (core::isEqual(element->getName(), name)) {
            element->setValue(value);
        }
    }
}

void core::CodeVisitor::visitLine(const std::string &startsWith, const std::string &line) {
    for (auto element : _lines) {
        if (core::startsWith(element->getLine(), startsWith)) {
            element->replace(line);
        }
    }
}

std::string core::CodeVisitor::build() {
    size_t i = 0;
    size_t size = _elements.size();
    std::stringstream stream;

    for (auto element : _elements) {
        stream << element->translate();
        if (i < size - 1) {
            stream << std::endl;
        }

        i++;
    }

    return stream.str();
}

void core::PixelArray::add(float x, float y) {
    _pixels.push_back(core::dto::Pixel(x, y));
    if (::isDistanceSmall(x) && ::isDistanceSmall(y)) {
        _texels.push_back(core::dto::Texel(static_cast<int>(::round(x)), static_cast<int>(::round(y))));
    }
    else if (::isDistanceSmall(x)) {
        _texels.push_back(getUpTexel(x, y));
        _texels.push_back(getDownTexel(x, y));
    }
    else if (::isDistanceSmall(y)) {
        _texels.push_back(getLeftTexel(x, y));
        _texels.push_back(getRightTexel(x, y));
    }
    else {
        _texels.push_back(getDownLeftTexel(x, y));
        _texels.push_back(getUpLeftTexel(x, y));
        _texels.push_back(getDownRightTexel(x, y));
        _texels.push_back(getUpRightTexel(x, y));
    }
}

std::string core::TexelCache::toInitializeVariables() {
    std::stringstream stream;
    for (auto i = 0; i < TexelCacheSize; i++) {
        stream << core::stringFormat(ENCRYPT_STRING("float4 %s = 0;"), toVariable(i).c_str()) << "\n";
    }

    return stream.str();
}

std::string core::Texel::toInitializeVariable(std::string variableName) {
    return core::stringFormat(
        ENCRYPT_STRING("%s = %s.Sample(%s, %s + float2(PIXEL_WIDTH * %d, PIXEL_HEIGHT * %d));"),
        variableName.c_str(),
        _textureName.c_str(),
        _samplerName.c_str(),
        _texcoord.c_str(),
        _texel.x,
        _texel.y);
}

std::string core::Texel::toGetValue(std::string variableName) {
    return core::stringFormat(
        ENCRYPT_STRING("%.5f * %s"),
        _weight,
        variableName.c_str());
}

std::string core::Pixel::toSetTextureCacheTexels(int map[TexelCacheSize]) {
    std::fill(map, map + TexelCacheSize, -1);
    std::stringstream stream;
    auto i = 0;

    for (auto it = _texels.begin(); it != _texels.end(); ++it) {
        auto index = _texelCache->indexOf((*it)._texel);
        if (index >= 0) {
            map[i++] = index;
            continue;
        }

        i++;
    }

    for (i = 0; i < _texels.size(); i++) {
        if (map[i] == -1) {
            auto taken = core::linq::makeEnumerable(map, TexelCacheSize).where([&](const int &index)->bool { return index >= 0; }).toList();
            auto j = _texelCache->getNextTexelIndex(taken);
            auto texel = *std::next(_texels.begin(), i);
            map[i] = j;
            _texelCache->setTexel(texel._texel, j);
            stream << texel.toInitializeVariable(_texelCache->toVariable(j)) << "\n";
        }
    }

    return stream.str();
}

std::string core::Pixel::toGetValue(std::string &value) {
    int map[TexelCacheSize];
    std::stringstream stream;
    stream << toSetTextureCacheTexels(map);

    int i = 0;
    value = "";
    for (auto it = _texels.begin(); it != _texels.end(); ++it) {
        auto texel = *it;
        auto j = map[i++];
        value += texel.toGetValue(_texelCache->toVariable(j));
        if (std::next(it) != _texels.end()) {
            value += ENCRYPT_STRING(" + ");
        }
    }

    return stream.str();
}

std::string core::Pixel::toGetTexelVariables(std::list<std::string> &variableNames) {
    int map[TexelCacheSize];
    std::stringstream stream;
    stream << toSetTextureCacheTexels(map);

    for (auto i = 0; i < _texels.size(); i++) {
        variableNames.push_back(_texelCache->toVariable(map[i]));
    }

    return stream.str();
}

std::string core::PixelsResult::toInitializeTexelCache() {
    return _texelCache->toInitializeVariables();
}

core::PixelsResult core::PixelArray::getPixels() {
    auto texels = core::linq::makeEnumerable(_texels)
        .distinct([&](const core::dto::Texel &first, const core::dto::Texel &second)->bool { return first.x == second.x && first.y == second.y; });

    core::PixelsResult result;
    result._texelCache.reset(new core::TexelCache(_texelName));
    for (auto pixel : _pixels) {
        if (::isDistanceSmall(pixel.x) && ::isDistanceSmall(pixel.y)) {
            auto centerTexel = core::dto::Texel(static_cast<int>(::round(pixel.x)), static_cast<int>(::round(pixel.y)));
            auto centerTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == centerTexel.x && texel.y == centerTexel.y; });
            auto firstTexel = core::Texel(centerTexel, _textureName, _samplerName, _texcoord, 1);
            result._texels.push_back(firstTexel);
            result._pixels.push_back(core::Pixel(result._texelCache, pixel, firstTexel));
        }
        else if (::isDistanceSmall(pixel.x)) {
            auto upTexel = getUpTexel(pixel.x, pixel.y);
            auto upTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == upTexel.x && texel.y == upTexel.y; });
            auto upTexelArea = ::getUpTexelArea(pixel.y);
            auto downTexel = getDownTexel(pixel.x, pixel.y);
            auto downTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == downTexel.x && texel.y == downTexel.y; });
            auto downTexelArea = ::getDownTexelArea(pixel.y);
            auto firstTexel = core::Texel(upTexel, _textureName, _samplerName, _texcoord, upTexelArea);
            auto secondTexel = core::Texel(downTexel, _textureName, _samplerName, _texcoord, downTexelArea);
            result._texels.push_back(firstTexel);
            result._texels.push_back(secondTexel);
            result._pixels.push_back(core::Pixel(result._texelCache, pixel, firstTexel, secondTexel));
        }
        else if (::isDistanceSmall(pixel.y)) {
            auto leftTexel = getLeftTexel(pixel.x, pixel.y);
            auto leftTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == leftTexel.x && texel.y == leftTexel.y; });
            auto leftTexelArea = ::getLeftTexelArea(pixel.x);
            auto rightTexel = getRightTexel(pixel.x, pixel.y);
            auto rightTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == rightTexel.x && texel.y == rightTexel.y; });
            auto rightTexelArea = ::getRightTexelArea(pixel.x);
            auto firstTexel = core::Texel(leftTexel, _textureName, _samplerName, _texcoord, leftTexelArea);
            auto secondTexel = core::Texel(rightTexel, _textureName, _samplerName, _texcoord, rightTexelArea);
            result._texels.push_back(firstTexel);
            result._texels.push_back(secondTexel);
            result._pixels.push_back(core::Pixel(result._texelCache, pixel, firstTexel, secondTexel));
        }
        else {
            auto downLeftTexel = getDownLeftTexel(pixel.x, pixel.y);
            auto downLeftTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == downLeftTexel.x && texel.y == downLeftTexel.y; });
            auto downLeftTexelArea = ::getDownLeftTexelArea(pixel.x, pixel.y);
            auto upLeftTexel = getUpLeftTexel(pixel.x, pixel.y);
            auto upLeftTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == upLeftTexel.x && texel.y == upLeftTexel.y; });
            auto upLeftTexelArea = ::getUpLeftTexelArea(pixel.x, pixel.y);
            auto downRightTexel = getDownRightTexel(pixel.x, pixel.y);
            auto downRightTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == downRightTexel.x && texel.y == downRightTexel.y; });
            auto downRightTexelArea = ::getDownRightTexelArea(pixel.x, pixel.y);
            auto upRightTexel = getUpRightTexel(pixel.x, pixel.y);
            auto upRightTexelIndex = texels.indexOf([&](const core::dto::Texel &texel)->bool { return texel.x == upRightTexel.x && texel.y == upRightTexel.y; });
            auto upRightTexelArea = ::getUpRightTexelArea(pixel.x, pixel.y);
            auto firstTexel = core::Texel(downLeftTexel, _textureName, _samplerName, _texcoord, downLeftTexelArea);
            auto secondTexel = core::Texel(upLeftTexel, _textureName, _samplerName, _texcoord, upLeftTexelArea);
            auto thirdTexel = core::Texel(downRightTexel, _textureName, _samplerName, _texcoord, downRightTexelArea);
            auto forthTexel = core::Texel(upRightTexel, _textureName, _samplerName, _texcoord, upRightTexelArea);
            result._texels.push_back(firstTexel);
            result._texels.push_back(secondTexel);
            result._texels.push_back(thirdTexel);
            result._texels.push_back(forthTexel);
            result._pixels.push_back(core::Pixel(result._texelCache, pixel, firstTexel, secondTexel, thirdTexel, forthTexel));
        }
    }

    result._texels = core::linq::makeEnumerable(result._texels)
        .distinct([&](const core::Texel &first, const core::Texel &second)->bool { return first.isEqual(second); })
        .toList();

    return result;
}

core::HlslTechniqueBuilder::HlslTechniqueBuilder(core::dto::Resolution resolution) {
    _resolution = resolution;
    _cachedResourceProvider.reset(new core::CachedCodeResourceProvider());
    _realResourceProvider.reset(new core::NoCodeFileResourceProvider());
}

core::HlslTechniqueBuilder::HlslTechniqueBuilder(core::dto::Resolution resolution, std::string directoryPath) {
    _resolution = resolution;
    _cachedResourceProvider.reset(new core::CachedCodeResourceProvider());
    _realResourceProvider.reset(new core::RealCodeFileResourceProvider(directoryPath));
}

void core::HlslTechniqueBuilder::applyBokehDoFPassType(
    ConcreteHlslTechniqueBuilder *builder,
    BokehDoFPassType passType,
    const core::dto::BokehDoF &bokehDoF) {
    float radians = 0;
    switch (passType) {
    case core::first:
        radians = ((float)M_PI / 180) * bokehDoF.shapeRotation;
        break;
    case core::second:
        radians = ((float)M_PI / 180) * (240 + bokehDoF.shapeRotation);
        break;
    case core::third:
        radians = ((float)M_PI / 180) * (120 + bokehDoF.shapeRotation);
        break;
    }

    PixelArray pixelArray(
        core::stringFormat(ENCRYPT_STRING("_tex_%d"), static_cast<int>(passType) % 2),
        ENCRYPT_STRING("_point_sampler"),
        ENCRYPT_STRING("input.texcoord"),
        ENCRYPT_STRING("texel"));

    for (unsigned int i = 1; i <= bokehDoF.shapeSize; i++) {
        pixelArray.add(i * cos(radians), i * sin(radians));
    }

    size_t count = 0;
    auto pixelsResult = pixelArray.getPixels();
    std::stringstream stream;
    stream << ENCRYPT_STRING("float4 pixelColor = 0;") << "\n";
    stream << pixelsResult.toInitializeTexelCache();

    for (auto pixelAggregate : pixelsResult._pixels) {
        std::string value;
        stream << pixelAggregate.toGetValue(value) << "\n";
        stream << ENCRYPT_STRING("pixelColor = ") << value << ";" << "\n";
        std::list<std::string> texelVariableNames;
        stream << pixelAggregate.toGetTexelVariables(texelVariableNames);
        count += texelVariableNames.size();

        if (bokehDoF.isPreservingShape)
        {
            for (auto texelVariableName : texelVariableNames) {
                auto texel = texelVariableName.c_str();
                stream << core::stringFormat(ENCRYPT_STRING(R"text(
if (%s.w >= bokehDoFCoC)
{
    bokehDoFCoC = %s.w;
})text"), texel, texel);
            }
        }

        stream << ENCRYPT_STRING("bokehDoFColor += pixelColor;") << "\n";
    }

    builder->add(new core::DefineAlterationElement(core::stringFormat(ENCRYPT_STRING("BOKEH_DOF_SHAPE_PASS_%d_TEXEL_COUNT"), static_cast<int>(passType)), ::translate(count)));
    builder->add(new core::LineAlterationElement(core::stringFormat(ENCRYPT_STRING("/* BOKEH_DOF_SHAPE_PASS_%d_PLACEHOLDER */"), static_cast<int>(passType)), stream.str()));
}

void core::HlslTechniqueBuilder::applyDepthBuffer(ConcreteHlslTechniqueBuilder *builder, const core::dto::DepthBuffer &depthBuffer) {
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_BUFFER_AVAILABLE"), ::translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_LINEAR_Z_NEAR"), core::toDefaultString(depthBuffer.linearZNear)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_LINEAR_Z_FAR"), core::toDefaultString(depthBuffer.linearZFar)));
    
    if (depthBuffer.isLimitEnabled) {
        builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_LIMIT_AVAILABLE"), ::translate(true)));
        builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_MIN"), core::toDefaultString(depthBuffer.depthMinimum)));
        builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_MAX"), core::toDefaultString(depthBuffer.depthMaximum)));
    }
}

std::unique_ptr<core::ConcreteHlslTechniqueBuilder> core::HlslTechniqueBuilder::getConcreteShaderCodeBuilder() {
    auto builder = new core::ConcreteHlslTechniqueBuilder(
        _cachedResourceProvider.get(),
        _realResourceProvider.get());

    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("SCREEN_WIDTH"), translate(_resolution.width)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("SCREEN_HEIGHT"), translate(_resolution.height)));

    return std::unique_ptr<core::ConcreteHlslTechniqueBuilder>(builder);
}

core::Technique core::HlslTechniqueBuilder::buildAlpha() {
    auto builder = getConcreteShaderCodeBuilder();

    core::Technique technique("technique::alpha", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psAlpha");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildBokehDoF(dto::BokehDoF bokehDoF, dto::DepthBuffer depthBuffer) {
    auto builder = getConcreteShaderCodeBuilder();
    auto gaussianBlur = ::getGaussianBlur(bokehDoF.blurStrength);

    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_ENABLED"), ::translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_DEPTH_MIN"), translate(bokehDoF.depthMinimum)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_DEPTH_MAX"), translate(bokehDoF.depthMaximum)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_DEPTH_RATE_GAIN"), translate(bokehDoF.depthRateGain)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_LUMINESCENCE_MIN"), translate(bokehDoF.luminescenceMinimum)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_LUMINESCENCE_MAX"), translate(bokehDoF.luminescenceMaximum)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_LUMINESCENCE_RATE_GAIN"), translate(bokehDoF.luminescenceRateGain)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_PRESERVE_SHAPE"), ::translate(bokehDoF.isPreservingShape)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_CHROMATIC_ABERRATION_ENABLED"), ::translate(bokehDoF.isChromaticAberrationEnabled)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_CHROMATIC_ABERRATION_FRINGE"), translate(bokehDoF.chromaticAberrationFringe)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_BLUR_STRENGTH"), translate(bokehDoF.shapeStrength)));
    
    applyBokehDoFPassType(builder.get(), core::BokehDoFPassType::first, bokehDoF);
    applyBokehDoFPassType(builder.get(), core::BokehDoFPassType::second, bokehDoF);
    applyBokehDoFPassType(builder.get(), core::BokehDoFPassType::third, bokehDoF);
    applyDepthBuffer(builder.get(), depthBuffer);

    const float minWeight = 0.001f;

    int length;
    float *offsets;
    float *weights;

    ::calculateGaussianBlur(
        gaussianBlur.size,
        gaussianBlur.sigma,
        minWeight,
        length,
        &offsets,
        &weights);

    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_BLUR_SIZE"), translate(length)));
    builder->add(new core::LineAlterationElement(ENCRYPT_STRING("static const float _bokeh_dof_blur_offset"), core::stringFormat(ENCRYPT_STRING("static const float _bokeh_dof_blur_offset[BOKEH_DOF_BLUR_SIZE] = { %s };"), core::join(ENCRYPT_STRING(", "), ::translate(offsets, length)).c_str())));
    builder->add(new core::LineAlterationElement(ENCRYPT_STRING("static const float _bokeh_dof_blur_weight"), core::stringFormat(ENCRYPT_STRING("static const float _bokeh_dof_blur_weight[BOKEH_DOF_BLUR_SIZE] = { %s };"), core::join(ENCRYPT_STRING(", "), ::translate(weights, length)).c_str())));

    delete[] offsets;
    delete[] weights;

    core::Texture tex0(core::TextureDataType::typefloat, _resolution);
    core::Texture tex1(core::TextureDataType::typefloat, _resolution);

    core::Technique technique("technique::bokehDoF", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    technique.textures[2] = tex0;
    technique.textures[3] = tex1;

    core::Pass pass0("vsMain", "psBokehDoFCoC");
    pass0.out.push_back(tex0.id);
    technique.passes.push_back(pass0);

    core::Pass pass1("vsMain", "psBokehDoFShapePass_0");
    pass1.in.push_back(tex0.id);
    pass1.out.push_back(tex1.id);
    technique.passes.push_back(pass1);

    core::Pass pass2("vsMain", "psBokehDoFShapePass_1");
    pass2.in.push_back(tex1.id);
    pass2.out.push_back(tex0.id);
    technique.passes.push_back(pass2);

    core::Pass pass3("vsMain", "psBokehDoFShapePass_2");
    pass3.in.push_back(tex0.id);
    pass3.out.push_back(tex1.id);
    technique.passes.push_back(pass3);

    if (bokehDoF.isBlurEnabled) {
        core::Pass pass4("vsMain", "psBokehDoFHorizontalGaussianBlur");
        pass4.in.push_back(tex1.id);
        pass4.out.push_back(tex0.id);
        technique.passes.push_back(pass4);

        core::Pass pass5("vsMain", "psBokehDoFVerticalGaussianBlur");
        pass5.in.push_back(tex0.id);
        pass5.out.push_back(tex1.id);
        technique.passes.push_back(pass5);
    }

    core::Pass pass6("vsMain", "psBokehDoFChromaticAberration");
    pass6.in.push_back(tex1.id);
    pass6.out.push_back(tex0.id);
    technique.passes.push_back(pass6);

    core::Pass pass7("vsMain", "psBokehDoFBlend");
    pass7.in.push_back(tex0.id);
    pass7.out.push_back(OutputTexId);
    technique.passes.push_back(pass7);
    
    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildDenoise(dto::Denoise denoise) {
    auto builder = getConcreteShaderCodeBuilder();
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_ENABLED"), translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_NOISE_LEVEL"), translate(denoise.noiseLevel)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_BLENDING_COEFFICIENT"), translate(denoise.blendingCoefficient)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_WEIGHT_THRESHOLD"), translate(denoise.weightThreshold)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_COUNTER_THRESHOLD"), translate(denoise.counterThreshold)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_GAUSSIAN_SIGMA"), translate(denoise.gaussianSigma)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_WINDOW_SIZE"), translate(denoise.windowSize)));

    core::Technique technique("technique::denoise", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psDenoise");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildDepth(dto::DepthBuffer depthBuffer) {
    auto builder = getConcreteShaderCodeBuilder();
    applyDepthBuffer(builder.get(), depthBuffer);

    core::Technique technique("technique::depth", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psDepth");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildGaussianBlur(dto::GaussianBlur gaussianBlur) {
    const float minWeight = 0.05f;

    int length;
    float *offsets;
    float *weights;

    calculateGaussianBlur(
        gaussianBlur.size,
        gaussianBlur.sigma,
        minWeight,
        length,
        &offsets,
        &weights);

    auto builder = getConcreteShaderCodeBuilder();
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("GAUSSIAN_BLUR_ENABLED"), translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("GAUSSIAN_BLUR_SIZE"), translate(length)));
    builder->add(new core::LineAlterationElement(ENCRYPT_STRING("static const float _gaussianblur_offset"), core::stringFormat(ENCRYPT_STRING("static const float _gaussianblur_offset[GAUSSIAN_BLUR_SIZE] = { %s };"), core::join(ENCRYPT_STRING(", "), ::translate(offsets, length)).c_str())));
    builder->add(new core::LineAlterationElement(ENCRYPT_STRING("static const float _gaussianblur_weight"), core::stringFormat(ENCRYPT_STRING("static const float _gaussianblur_weight[GAUSSIAN_BLUR_SIZE] = { %s };"), core::join(ENCRYPT_STRING(", "), ::translate(weights, length)).c_str())));

    delete[] offsets;
    delete[] weights;

    core::Texture tex(core::TextureDataType::typefloat, _resolution);

    core::Technique technique("technique::gaussianBlur", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    technique.textures[2] = tex;

    core::Pass pass0("vsMain", "psHorizontalGaussianBlur");
    pass0.out.push_back(tex.id);
    technique.passes.push_back(pass0);

    core::Pass pass1("vsMain", "psVerticalGaussianBlur");
    pass1.in.push_back(tex.id);
    pass1.out.push_back(OutputTexId);
    technique.passes.push_back(pass1);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildHDR(dto::HDR hdr) {
    auto builder = getConcreteShaderCodeBuilder();
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("HDR_ENABLED"), translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("HDR_STRENGTH"), translate(hdr.strength)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("HDR_RADIUS_2"), translate(hdr.radius)));

    core::Technique technique("technique::hdr", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psHDR");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildLiftGammaGain(dto::LiftGammaGain liftGammaGain) {
    auto builder = getConcreteShaderCodeBuilder();
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_ENABLED"), translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_LIFT_RED"), translate(liftGammaGain.lift.red)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_LIFT_GREEN"), translate(liftGammaGain.lift.green)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_LIFT_BLUE"), translate(liftGammaGain.lift.blue)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAMMA_RED"), translate(liftGammaGain.gamma.red)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAMMA_GREEN"), translate(liftGammaGain.gamma.green)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAMMA_BLUE"), translate(liftGammaGain.gamma.blue)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAIN_RED"), translate(liftGammaGain.gain.red)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAIN_GREEN"), translate(liftGammaGain.gain.green)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAIN_BLUE"), translate(liftGammaGain.gain.blue)));

    core::Technique technique("technique::liftGammaGain", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psLiftGammaGain");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildLumaSharpen(dto::LumaSharpen lumaSharpen) {
    auto builder = getConcreteShaderCodeBuilder();
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LUMASHARPEN_ENABLED"), translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LUMASHARPEN_SHARPENING_STRENGTH"), translate(lumaSharpen.sharpeningStrength)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LUMASHARPEN_SHARPENING_CLAMP"), translate(lumaSharpen.sharpeningClamp)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("LUMASHARPEN_OFFSET"), translate(lumaSharpen.offset)));

    core::Technique technique("technique::lumaSharpen", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psLumaSharpen");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildLuminescence() {
    auto builder = getConcreteShaderCodeBuilder();

    core::Technique technique("technique::luminescence", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psLuminescence");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildTonemap(dto::Tonemap tonemap) {
    auto builder = getConcreteShaderCodeBuilder();
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_ENABLED"), translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_GAMMA"), translate(tonemap.gamma)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_EXPOSURE"), translate(tonemap.exposure)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_SATURATION"), translate(tonemap.saturation)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_BLEACH"), translate(tonemap.bleach)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_DEFOG"), translate(tonemap.defog)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_FOG_RED"), translate(tonemap.fog.red)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_FOG_GREEN"), translate(tonemap.fog.green)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_FOG_BLUE"), translate(tonemap.fog.blue)));

    core::Technique technique("technique::tonemap", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psTonemap");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

core::Technique core::HlslTechniqueBuilder::buildVibrance(dto::Vibrance vibrance) {
    auto builder = getConcreteShaderCodeBuilder();
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_ENABLED"), translate(true)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_STRENGTH"), translate(vibrance.strength)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_GAIN_RED"), translate(vibrance.gain.red)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_GAIN_GREEN"), translate(vibrance.gain.green)));
    builder->add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_GAIN_BLUE"), translate(vibrance.gain.blue)));

    core::Technique technique("technique::vibrance", core::TechniqueCode(builder->buildVertexShaderCode(), builder->buildPixelShaderCode()));
    core::Pass pass("vsMain", "psVibrance");
    pass.out.push_back(OutputTexId);
    technique.passes.push_back(pass);

    return technique;
}

void core::ConcreteHlslTechniqueBuilder::add(core::IAlterationElement *element) {
    _elements.push_back(std::shared_ptr<IAlterationElement>(element));
}

std::string core::ConcreteHlslTechniqueBuilder::getResource(const std::string &key) {
    try {
        log(debug, core::stringFormat(ENCRYPT_STRING("getting file: %s..."), key.c_str()));
        return _realResourceProvider->get(key);
    }
    catch (core::exception::ResourceNotFoundException const &exception) {
        log(debug, core::stringFormat(ENCRYPT_STRING("file not found exception occured: %s"), exception.what()));
        log(debug, core::stringFormat(ENCRYPT_STRING("getting: %s resource from cache..."), key.c_str()));
        return _cachedResourceProvider->get(key);
    }
}

std::string core::ConcreteHlslTechniqueBuilder::buildPixelShaderCode() {
    core::CodeVisitor codeVisitor(getResource(ENCRYPT_STRING("pixelshader.hlsl")));
    for (auto element : _elements) {
        element->accept(codeVisitor);
    }

    return codeVisitor.build();
}

std::string core::ConcreteHlslTechniqueBuilder::buildVertexShaderCode() {
    core::CodeVisitor codeVisitor(getResource(ENCRYPT_STRING("vertexshader.hlsl")));
    for (auto element : _elements) {
        element->accept(codeVisitor);
    }

    return codeVisitor.build();
}

core::HlslTechniqueFactory::HlslTechniqueFactory(
    core::HlslTechniqueBuilder *hlslTechniqueBuilder,
    core::ISerializer *serializer,
    core::dto::DepthBuffer depthBuffer) {
    _hlslTechniqueBuilder = hlslTechniqueBuilder;
    _serializer = serializer;
    _depthBuffer = depthBuffer;
    _policies = {
        { core::TechniqueType::bokehdof, [&](const std::string &data) { return _hlslTechniqueBuilder->buildBokehDoF(_serializer->deserializeBokehDoF(data), _depthBuffer); } },
        { core::TechniqueType::denoise, [&](const std::string &data) { return _hlslTechniqueBuilder->buildDenoise(_serializer->deserializeDenoise(data)); } },
        { core::TechniqueType::hdr, [&](const std::string &data) { return _hlslTechniqueBuilder->buildHDR(_serializer->deserializeHDR(data)); } },
        { core::TechniqueType::liftgammagain, [&](const std::string &data) { return _hlslTechniqueBuilder->buildLiftGammaGain(_serializer->deserializeLiftGammaGain(data)); } },
        { core::TechniqueType::lumasharpen, [&](const std::string &data) { return _hlslTechniqueBuilder->buildLumaSharpen(_serializer->deserializeLumaSharpen(data)); } },
        { core::TechniqueType::tonemap, [&](const std::string &data) { return _hlslTechniqueBuilder->buildTonemap(_serializer->deserializeTonemap(data)); } },
        { core::TechniqueType::vibrance, [&](const std::string &data) { return _hlslTechniqueBuilder->buildVibrance(_serializer->deserializeVibrance(data)); } }
    };
}

core::Technique core::HlslTechniqueFactory::create(core::TechniqueType type, const std::string &data) {
    auto iterator = _policies.find(type);
    if (iterator == _policies.end()) {
        throw core::exception::OperationException(
            ENCRYPT_STRING("core::HlslTechniqueFactory"),
            core::stringFormat(ENCRYPT_STRING("technique: %d could not be created"), static_cast<int>(type)));
    }

    return iterator->second(data);
}