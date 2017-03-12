#include "stdafx.h"
#include "codebuilder.h"
#include "resourceprovider.h"

using namespace core::logging;

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

void core::BokehDoFCodeBuilder::build() {
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_ENABLED"), ::translate(true)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_DEPTH_MIN"), translate(_depthMinimum)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_DEPTH_MAX"), translate(_depthMaximum)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_DEPTH_RATE_GAIN"), translate(_depthRateGain)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_LUMINESCENCE_MIN"), translate(_luminescenceMinimum)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_LUMINESCENCE_MAX"), translate(_luminescenceMaximum)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_LUMINESCENCE_RATE_GAIN"), translate(_luminescenceRateGain)));
}

void core::BokehDoFCodeBuilder::buildBokehDoFPassType(BokehDoFPassType passType, bool preserveShape, unsigned int size, float rotation) {
    float radians = 0;

    switch (passType) {
    case core::first:
        radians = ((float)M_PI / 180) * rotation;
        break;
    case core::second:
        radians = ((float)M_PI / 180) * (240 + rotation);
        break;
    case core::third:
        radians = ((float)M_PI / 180) * (120 + rotation);
        break;
    }

    PixelArray pixelArray(ENCRYPT_STRING("_pass_0_texture"), ENCRYPT_STRING("_point_sampler"), ENCRYPT_STRING("input.texcoord"), ENCRYPT_STRING("texel"));
    for (unsigned int i = 1; i <= size; i++) {
        pixelArray.add(i * cos(radians), i * sin(radians));
    }

    size_t count = 0;
    auto pixelsResult = pixelArray.getPixels();
    std::stringstream stream;
    stream << pixelsResult.toInitializeTexelCache();

    for (auto pixelAggregate : pixelsResult._pixels) {
        std::string value;
        stream << pixelAggregate.toGetValue(value) << "\n";
        stream << ENCRYPT_STRING("pixelColor = ") << value << ";" << "\n";
        std::list<std::string> texelVariableNames;
        stream << pixelAggregate.toGetTexelVariables(texelVariableNames);
        count += texelVariableNames.size();

        if (preserveShape)
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

    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_ENABLED"), ::translate(true)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_TEXEL_COUNT"), ::translate(count)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_PRESERVE_SHAPE"), ::translate(preserveShape)));
    _codeBuilder->add(new core::LineAlterationElement(ENCRYPT_STRING("/* BOKEH DOF: PLACEHOLDER */"), stream.str()));
}

void core::BokehDoFCodeBuilder::buildCoC() {
    build();
}

void core::BokehDoFCodeBuilder::buildBlur(BokehDoFPassType passType, bool isPreservingShape, unsigned int size, float rotation) {
    build();
    buildBokehDoFPassType(passType, isPreservingShape, size, rotation);
}

void core::BokehDoFCodeBuilder::buildChromaticAberration(float fringe) {
    build();
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_CHROMATIC_ABERRATION_ENABLED"), ::translate(true)));
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_CHROMATIC_ABERRATION_FRINGE"), translate(fringe)));
}

void core::BokehDoFCodeBuilder::buildBlend(float strength) {
    build();
    _codeBuilder->add(new core::DefineAlterationElement(ENCRYPT_STRING("BOKEH_DOF_BLUR_STRENGTH"), translate(strength)));
}

void core::BokehDoFCodeBuilder::enable() {
    build();
}

core::ShaderCodeBuilder::ShaderCodeBuilder() {
    _cachedResourceProvider.reset(new core::CachedCodeResourceProvider());
    _realResourceProvider.reset(new core::NoCodeFileResourceProvider());
}

core::ShaderCodeBuilder::ShaderCodeBuilder(std::string directoryPath) {
    _cachedResourceProvider.reset(new core::CachedCodeResourceProvider());
    _realResourceProvider.reset(new core::RealCodeFileResourceProvider(directoryPath));
}

void core::ShaderCodeBuilder::add(core::IAlterationElement *element) {
    _elements.push_back(std::shared_ptr<IAlterationElement>(element));
}

std::string core::ShaderCodeBuilder::getResource(const std::string &key) {
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

void core::ShaderCodeBuilder::setLinearDepthTextureAccessibility(float distanceNear, float distanceFar) {
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_BUFFER_AVAILABLE"), ::translate(true)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_LINEAR_Z_NEAR"), core::toDefaultString(distanceNear)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_LINEAR_Z_FAR"), core::toDefaultString(distanceFar)));
}

void core::ShaderCodeBuilder::setDepthTextureLimit(float depthMin, float depthMax) {
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_LIMIT_AVAILABLE"), ::translate(true)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_MIN"), core::toDefaultString(depthMin)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DEPTH_MAX"), core::toDefaultString(depthMax)));
}

core::BokehDoFCodeBuilder core::ShaderCodeBuilder::setBokehDoF(float depthMinimum, float depthMaximum, float depthRateGain, float luminescenceMinimum, float luminescenceMaximum, float luminescenceRateGain) {
    return core::BokehDoFCodeBuilder(this, depthMinimum, depthMaximum, depthRateGain, luminescenceMinimum, luminescenceMaximum, luminescenceRateGain);
}

void core::ShaderCodeBuilder::setDenoise(float noiseLevel, float blendingCoefficient, float weightThreshold, float counterThreshold, float gaussianSigma, unsigned int windowSize) {
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_ENABLED"), translate(true)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_NOISE_LEVEL"), translate(noiseLevel)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_BLENDING_COEFFICIENT"), translate(blendingCoefficient)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_WEIGHT_THRESHOLD"), translate(weightThreshold)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_COUNTER_THRESHOLD"), translate(counterThreshold)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_GAUSSIAN_SIGMA"), translate(gaussianSigma)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("DENOISE_WINDOW_SIZE"), translate(windowSize)));
}

void core::ShaderCodeBuilder::setLiftGammaGain(core::dto::Color lift, core::dto::Color gamma, core::dto::Color gain) {
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_ENABLED"), translate(true)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_LIFT_RED"), translate(lift.red)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_LIFT_GREEN"), translate(lift.green)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_LIFT_BLUE"), translate(lift.blue)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAMMA_RED"), translate(gamma.red)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAMMA_GREEN"), translate(gamma.green)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAMMA_BLUE"), translate(gamma.blue)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAIN_RED"), translate(gain.red)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAIN_GREEN"), translate(gain.green)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LIFTGAMMAGAIN_GAIN_BLUE"), translate(gain.blue)));
}

void core::ShaderCodeBuilder::setLumaSharpen(float sharpeningStrength, float sharpeningClamp, float offset) {
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LUMASHARPEN_ENABLED"), translate(true)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LUMASHARPEN_SHARPENING_STRENGTH"), translate(sharpeningStrength)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LUMASHARPEN_SHARPENING_CLAMP"), translate(sharpeningClamp)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("LUMASHARPEN_OFFSET"), translate(offset)));
}

void core::ShaderCodeBuilder::setResolution(unsigned int width, unsigned int height) {
    add(new core::DefineAlterationElement(ENCRYPT_STRING("SCREEN_WIDTH"), translate(width)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("SCREEN_HEIGHT"), translate(height)));
}

void core::ShaderCodeBuilder::setTonemap(float gamma, float exposure, float saturation, float bleach, float defog, core::dto::Color fog) {
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_ENABLED"), translate(true)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_GAMMA"), translate(gamma)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_EXPOSURE"), translate(exposure)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_SATURATION"), translate(saturation)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_BLEACH"), translate(bleach)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_DEFOG"), translate(defog)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_FOG_RED"), translate(fog.red)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_FOG_GREEN"), translate(fog.green)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("TONEMAP_FOG_BLUE"), translate(fog.blue)));
}

void core::ShaderCodeBuilder::setVibrance(float strength, core::dto::Color gain) {
    add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_ENABLED"), translate(true)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_STRENGTH"), translate(strength)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_GAIN_RED"), translate(gain.red)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_GAIN_GREEN"), translate(gain.green)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("VIBRANCE_GAIN_BLUE"), translate(gain.blue)));
}

void core::ShaderCodeBuilder::setGaussianBlur(unsigned int size, float sigma, float minWeight) {
    int length;
    float *offsets;
    float *weights;

    calculateGaussianBlur(size, sigma, minWeight, length, &offsets, &weights);

    add(new core::DefineAlterationElement(ENCRYPT_STRING("GAUSSIAN_BLUR_ENABLED"), translate(true)));
    add(new core::DefineAlterationElement(ENCRYPT_STRING("GAUSSIAN_BLUR_SIZE"), translate(length)));
    add(new core::LineAlterationElement(ENCRYPT_STRING("static const float _gaussianblur_offset"), core::stringFormat(ENCRYPT_STRING("static const float _gaussianblur_offset[GAUSSIAN_BLUR_SIZE] = { %s };"), core::join(ENCRYPT_STRING(", "), ::translate(offsets, length)).c_str())));
    add(new core::LineAlterationElement(ENCRYPT_STRING("static const float _gaussianblur_weight"), core::stringFormat(ENCRYPT_STRING("static const float _gaussianblur_weight[GAUSSIAN_BLUR_SIZE] = { %s };"), core::join(ENCRYPT_STRING(", "), ::translate(weights, length)).c_str())));

    delete[] offsets;
    delete[] weights;
}

std::string core::ShaderCodeBuilder::buildPixelShaderCode() {
    core::CodeVisitor codeVisitor(getResource(ENCRYPT_STRING("pixelshader.hlsl")));
    for (auto element : _elements) {
        element->accept(codeVisitor);
    }

    return codeVisitor.build();
}

std::string core::ShaderCodeBuilder::buildVertexShaderCode() {
    core::CodeVisitor codeVisitor(getResource(ENCRYPT_STRING("vertexshader.hlsl")));
    for (auto element : _elements) {
        element->accept(codeVisitor);
    }

    return codeVisitor.build();
}