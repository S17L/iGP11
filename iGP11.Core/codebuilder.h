#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
    namespace dto {
        struct Texel final {
            int x = 0;
            int y = 0;
            Texel() {}
            Texel(int x, int y) {
                this->x = x;
                this->y = y;
            }
            bool isEqual(const Texel &other) const {
                return x == other.x && y == other.y;
            }
        };

        struct Pixel final {
            float x = 0;
            float y = 0;
            Pixel() {}
            Pixel(float x, float y) {
                this->x = x;
                this->y = y;
            }
        };
    }

    static const unsigned int TexelCacheSize = 4;

    enum BokehDoFPassType {
        first = 0,
        second = 1,
        third = 2
    };

    class IElement {
    public:
        virtual ~IElement() {}
        virtual std::string translate() = 0;
    };

    class DefineElement final : public IElement {
    private:
        std::string _name;
        std::string _value;
    public:
        DefineElement(const std::string &name, const std::string &value)
            : _name(name), _value(value) {}
        virtual ~DefineElement() {}
        virtual std::string translate() override;
        std::string getName() {
            return _name;
        }
        void setValue(const std::string &value) {
            _value = value;
        }
    };

    class LineElement final : public IElement {
    private:
        std::string _line;
    public:
        LineElement(const std::string &line)
            : _line(line) {}
        virtual std::string translate() override {
            return _line;
        };
        std::string getLine() {
            return _line;
        }
        void replace(const std::string &line) {
            _line = line;
        }
    };

    class CodeVisitor final {
    private:
        std::list<std::shared_ptr<DefineElement>> _defines;
        std::list<std::shared_ptr<LineElement>> _lines;
        std::list<std::shared_ptr<IElement>> _elements;
    public:
        CodeVisitor(const std::string &code);
        void visitDefine(const std::string &name, const std::string &value);
        void visitLine(const std::string &startsWith, const std::string &line);
        std::string build();
    };

    class IAlterationElement {
    public:
        virtual ~IAlterationElement() {}
        virtual void accept(CodeVisitor &visitor) = 0;
    };

    class DefineAlterationElement final : public IAlterationElement {
    private:
        std::string _name;
        std::string _value;
    public:
        DefineAlterationElement(const std::string &name, const std::string &value)
            : _name(name), _value(value) {}
        virtual ~DefineAlterationElement() {}
        virtual void accept(CodeVisitor &visitor) override {
            visitor.visitDefine(_name, _value);
        }
    };

    class LineAlterationElement final : public IAlterationElement {
    private:
        std::string _startsWith;
        std::string _line;
    public:
        LineAlterationElement(const std::string &startsWith, const std::string &line)
            : _startsWith(startsWith), _line(line) {}
        virtual ~LineAlterationElement() {}
        virtual void accept(CodeVisitor &visitor) override {
            visitor.visitLine(_startsWith, _line);
        }
    };

    class TexelCache final {
        friend class Pixel;
        friend class PixelArray;
        friend class PixelsResult;
        friend class Texel;
    private:
        core::Nullable<dto::Texel> _texels[TexelCacheSize] = {};
        unsigned int _lastTexelIndex = -1;
        std::string _variableName;
        TexelCache(std::string variableName)
            : _variableName(variableName) {}
        unsigned int iterateTexelIndex(unsigned int index) {
            return index >= TexelCacheSize - 1 ? 0 : index + 1;
        }
        void setTexel(dto::Texel texel, unsigned int index) {
            if (index >= TexelCacheSize) {
                throw core::exception::OperationException(ENCRYPT_STRING("core::texelcache"), ENCRYPT_STRING("index is out of range"));
            }

            _lastTexelIndex = index;
            _texels[index].set(texel);
        }
        unsigned int indexOf(const dto::Texel &texel) {
            return core::linq::makeEnumerable(_texels, TexelCacheSize)
                .indexOf([&](const core::Nullable<dto::Texel> &item)->bool { return item.hasValue() && item->isEqual(texel); });
        }
        unsigned int getNextTexelIndex(std::list<int> omit) {
            auto nextTexelIndex = iterateTexelIndex(_lastTexelIndex);
            auto omitEnumerable = core::linq::makeEnumerable(omit);

            for (auto i = 0; i < TexelCacheSize; i++) {
                if (omitEnumerable.contains([&](const int &index)->bool { return index == nextTexelIndex; })) {
                    nextTexelIndex = iterateTexelIndex(nextTexelIndex);
                }
                else {
                    return nextTexelIndex;
                }
            }

            return -1;
        }
        std::string toVariable(unsigned int index) {
            return core::stringFormat("%s_%u", _variableName.c_str(), index);
        }
        std::string toInitializeVariables();
    };

    class Texel final {
        friend class Pixel;
        friend class PixelArray;
    private:
        dto::Texel _texel;
        std::string _textureName;
        std::string _samplerName;
        std::string _texcoord;
        float _weight;
    public:
        Texel(dto::Texel texel, std::string textureName, std::string samplerName, std::string texcoord, float weight)
            : _texel(texel), _textureName(textureName), _samplerName(samplerName), _texcoord(texcoord), _weight(weight) {}
        bool isEqual(const Texel &other) const {
            return _texel.isEqual(other._texel);
        }
        std::string toInitializeVariable(std::string variableName);
        std::string toGetValue(std::string variableName);
    };

    class Pixel final {
    private:
        std::shared_ptr<TexelCache> _texelCache;
        dto::Pixel _pixel;
        std::list<Texel> _texels;
        std::string toSetTextureCacheTexels(int map[TexelCacheSize]);
    public:
        Pixel(std::shared_ptr<TexelCache> texelCache, dto::Pixel pixel, Texel first) {
            _texelCache = texelCache;
            _pixel = pixel;
            _texels.push_back(first);
        }
        Pixel(std::shared_ptr<TexelCache> texelCache, dto::Pixel pixel, Texel first, Texel second) {
            _texelCache = texelCache;
            _pixel = pixel;
            _texels.push_back(first);
            _texels.push_back(second);
        }
        Pixel(std::shared_ptr<TexelCache> texelCache, dto::Pixel pixel, Texel first, Texel second, Texel third, Texel forth) {
            _texelCache = texelCache;
            _pixel = pixel;
            _texels.push_back(first);
            _texels.push_back(second);
            _texels.push_back(third);
            _texels.push_back(forth);
        }
        std::list<Texel> getTexels() const {
            return _texels;
        }
        std::string toGetValue(std::string &value);
        std::string toGetTexelVariables(std::list<std::string> &variableNames);
    };

    class PixelsResult {
        friend class PixelArray;
    private:
        std::shared_ptr<TexelCache> _texelCache;
    public:
        std::list<Pixel> _pixels;
        std::list<Texel> _texels;
        std::string toInitializeTexelCache();
    };

    class PixelArray final {
    private:
        std::string _textureName;
        std::string _samplerName;
        std::string _texcoord;
        std::string _texelName;
        std::list<dto::Pixel> _pixels;
        std::list<dto::Texel> _texels;
    public:
        PixelArray(std::string textureName, std::string samplerName, std::string texcoord, std::string texelName)
            : _textureName(textureName), _samplerName(samplerName), _texcoord(texcoord), _texelName(texelName) {}
        void add(float x, float y);
        PixelsResult getPixels();
    };

    class ShaderCodeBuilder final {
        friend class BokehDoFCodeBuilder;
    private:
        std::unique_ptr<IResourceProvider> _cachedResourceProvider;
        std::unique_ptr<IResourceProvider> _realResourceProvider;
        std::list<std::shared_ptr<IAlterationElement>> _elements;
        void add(IAlterationElement *element);
        std::string getResource(const std::string &key);
    public:
        ShaderCodeBuilder();
        ShaderCodeBuilder(std::string directoryPath);
        void setLinearDepthTextureAccessibility(float distanceNear, float distanceFar);
        void setDepthTextureLimit(float depthMin, float depthMax);
        BokehDoFCodeBuilder setBokehDoF(float depthMinimum, float depthMaximum, float depthRateGain, float luminescenceMinimum, float luminescenceMaximum, float luminescenceRateGain);
        void setDenoise(float noiseLevel, float blendingCoefficient, float weightThreshold, float counterThreshold, float gaussianSigma, unsigned int windowSize);
        void setGaussianBlur(unsigned int size, float sigma, float minWeight = 0.005);
        void setLiftGammaGain(dto::Color lift, dto::Color gamma, dto::Color gain);
        void setLumaSharpen(float sharpeningStrength, float sharpeningClamp, float offset);
        void setResolution(unsigned int width, unsigned int height);
        void setTonemap(float gamma, float exposure, float saturation, float bleach, float defog, dto::Color fog);
        void setVibrance(float strength, dto::Color gain);
        std::string buildPixelShaderCode();
        std::string buildVertexShaderCode();
    };

    class BokehDoFCodeBuilder final {
    private:
        ShaderCodeBuilder *_codeBuilder;
        float _depthMinimum;
        float _depthMaximum;
        float _depthRateGain;
        float _luminescenceMinimum;
        float _luminescenceMaximum;
        float _luminescenceRateGain;
        void build();
        void buildBokehDoFPassType(BokehDoFPassType passType, bool isPreservingShape, unsigned int size, float rotation);
    public:
        BokehDoFCodeBuilder(ShaderCodeBuilder *codeBuilder, float depthMinimum, float depthMaximum, float depthRateGain, float luminescenceMinimum, float luminescenceMaximum, float luminescenceRateGain)
            : _codeBuilder(codeBuilder), _depthMinimum(depthMinimum), _depthMaximum(depthMaximum), _depthRateGain(depthRateGain), _luminescenceMinimum(luminescenceMinimum), _luminescenceMaximum(luminescenceMaximum), _luminescenceRateGain(luminescenceRateGain) {}
        void buildCoC();
        void buildBlur(BokehDoFPassType passType, bool isPreservingShape, unsigned int size, float rotation);
        void buildChromaticAberration(float fringe);
        void buildBlend(float strength);
        void enable();
    };
}