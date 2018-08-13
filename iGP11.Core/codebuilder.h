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

    typedef unsigned int type_slot;
    typedef unsigned int type_tex_id;
    static const type_tex_id OutputTexId = 0;
    static type_tex_id _tex_id = 1;
    inline type_tex_id nextTexId() {
        return _tex_id++;
    }

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
        float getX() {
            return _pixel.x;
        }
        float getY() {
            return _pixel.y;
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

    enum class TextureDataType {
        typefloat = 0
    };

    struct Texture final {
        type_tex_id id = nextTexId();
        TextureDataType dataType;
        dto::Resolution resolution;
        Texture() {}
        Texture(TextureDataType dataType, dto::Resolution resolution) {
            this->dataType = dataType;
            this->resolution = resolution;
        }
    };

    struct Pass {
        std::string vsFunctionName;
        std::string psFunctionName;
        std::list<type_tex_id> in;
        std::list<type_tex_id> out;
        Pass(
            std::string vsFunctionName,
            std::string psFunctionName) {
            this->vsFunctionName = vsFunctionName;
            this->psFunctionName = psFunctionName;
        }
    };

    struct TechniqueCode final {
        std::string vsCode;
        std::string psCode;
        TechniqueCode() {}
        TechniqueCode(std::string vsCode, std::string psCode) {
            this->vsCode = vsCode;
            this->psCode = psCode;
        }
    };

    struct Technique {
        std::string name;
        TechniqueCode code;
        type_slot color = 0;
        type_slot depth = 1;
        std::list<Pass> passes;
        std::map<type_slot, Texture> textures;
        Technique() {}
        Technique(
            std::string name,
            TechniqueCode code) {
            this->name = name;
            this->code = code;
        }
    };

    class ConcreteHlslTechniqueBuilder final {
    private:
        IResourceProvider *_cachedResourceProvider;
        IResourceProvider *_realResourceProvider;
        std::list<std::shared_ptr<IAlterationElement>> _elements;
        std::string getResource(const std::string &key);
    public:
        ConcreteHlslTechniqueBuilder(IResourceProvider *cachedResourceProvider, IResourceProvider *realResourceProvider)
            : _cachedResourceProvider(cachedResourceProvider), _realResourceProvider(realResourceProvider) {}
        void add(IAlterationElement *element);
        std::string buildPixelShaderCode();
        std::string buildVertexShaderCode();
    };

    class HlslTechniqueBuilder final {
    private:
        dto::Resolution _resolution;
        std::unique_ptr<IResourceProvider> _cachedResourceProvider;
        std::unique_ptr<IResourceProvider> _realResourceProvider;
        void applyBokehDoFPassType(ConcreteHlslTechniqueBuilder *builder, BokehDoFPassType pass, const dto::BokehDoF &bokehDoF);
        void applyDepthBuffer(ConcreteHlslTechniqueBuilder *builder, const dto::DepthBuffer &depthBuffer);
        std::unique_ptr<ConcreteHlslTechniqueBuilder> getConcreteShaderCodeBuilder();
    public:
        HlslTechniqueBuilder(dto::Resolution resolution);
        HlslTechniqueBuilder(dto::Resolution resolution, std::string directoryPath);
        Technique buildAlpha();
        Technique buildBokehDoF(dto::BokehDoF bokehDoF, dto::DepthBuffer depthBuffer);
        Technique buildDenoise(dto::Denoise denoise);
        Technique buildDepth(dto::DepthBuffer depthBuffer);
        Technique buildGaussianBlur(dto::GaussianBlur gaussianBlur);
        Technique buildHDR(dto::HDR hdr);
        Technique buildLiftGammaGain(dto::LiftGammaGain liftGammaGain);
        Technique buildLumaSharpen(dto::LumaSharpen);
        Technique buildLuminescence();
        Technique buildTonemap(dto::Tonemap tonemap);
        Technique buildVibrance(dto::Vibrance vibrance);
    };

    typedef std::function<Technique(const std::string &data)> TechniqueFactoryFunction;

    class HlslTechniqueFactory final {
    private:
        HlslTechniqueBuilder *_hlslTechniqueBuilder;
        core::ISerializer *_serializer;
        dto::DepthBuffer _depthBuffer;
        std::map<core::TechniqueType, TechniqueFactoryFunction> _policies;
    public:
        HlslTechniqueFactory(
            HlslTechniqueBuilder *hlslTechniqueBuilder,
            core::ISerializer *serializer,
            dto::DepthBuffer depthBuffer);
        virtual ~HlslTechniqueFactory() {}
        virtual Technique create(core::TechniqueType type, const std::string &data);
    };
}