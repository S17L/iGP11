#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "camera.h"
#include "codebuilder.h"
#include "logger.h"
#include "state.h"
#include "utility.h"

using namespace core::logging;

namespace direct3d11 {
    namespace dto {
        struct MatrixBufferType {
            DirectX::XMFLOAT4X4 world;
            DirectX::XMFLOAT4X4 view;
            DirectX::XMFLOAT4X4 projection;
        };

        struct VertexType {
            DirectX::XMFLOAT3 position;
            DirectX::XMFLOAT2 texture;
        };
    }

    class CustomizableTexture : public ITexture {
    private:
        Direct3D11Context *_context;
        ID3D11Texture2D *_texture;
        ID3D11ShaderResourceView *_textureShaderView;
        ID3D11RenderTargetView *_textureRenderView;
    public:
        CustomizableTexture(Direct3D11Context *context, ID3D11Texture2D *texture, ID3D11ShaderResourceView *textureShaderView, ID3D11RenderTargetView *textureRenderView)
            : _context(context), _texture(texture), _textureShaderView(textureShaderView), _textureRenderView(textureRenderView) {}
        virtual ~CustomizableTexture() {};
        virtual ID3D11Texture2D* get() const override {
            return _texture;
        }
        virtual ID3D11ShaderResourceView* getInView() const override {
            return _textureShaderView;
        }
        virtual ID3D11RenderTargetView* getOutView() const override {
            return _textureRenderView;
        }
    };

    class Texture : public ITexture {
    private:
        Direct3D11Context *_context;
        core::disposing::unique_ptr<ID3D11Texture2D> _texture;
        core::disposing::unique_ptr<ID3D11ShaderResourceView> _textureShaderView;
        core::disposing::unique_ptr<ID3D11RenderTargetView> _textureRenderView;
    public:
        Texture(Direct3D11Context *context, const D3D11_TEXTURE2D_DESC &description);
        virtual ID3D11Texture2D* get() const override {
            return _texture.get();
        }
        virtual ID3D11ShaderResourceView* getInView() const override {
            return _textureShaderView.get();
        }
        virtual ID3D11RenderTargetView* getOutView() const override {
            return _textureRenderView.get();
        }
    };

    class RenderingProxy {
    private:
        unsigned int _count = 0;
        unsigned int _index = 0;
        bool _last = false;
        Direct3D11Context *_context;
        ID3D11Texture2D *_color;
        ID3D11Texture2D *_depth;
        std::unique_ptr<CustomizableTexture> _renderingTexture;
        std::unique_ptr<ITexture> _colorTexture;
        std::unique_ptr<ITexture> _chainFirstTexture;
        std::unique_ptr<ITexture> _chainSecondTexture;
        core::disposing::unique_ptr<ID3D11Texture2D> _depthTexture;
        core::disposing::unique_ptr<ID3D11ShaderResourceView> _depthTextureView;
        core::disposing::unique_ptr<ID3D11RenderTargetView> _inputRenderTargetView;
        core::disposing::unique_ptr<ID3D11RasterizerState> _rasterizerState;
        core::disposing::unique_ptr<ID3D11DepthStencilState> _depthStencilState;
        std::list<std::shared_ptr<direct3d11::IState>> _states;
    public:
        RenderingProxy(Direct3D11Context *context, ID3D11Texture2D *color, ID3D11Texture2D *depth);
        virtual ~RenderingProxy() {}
        void begin();
        void end();
        ITexture* iterateIn();
        ITexture* iterateOut(bool last);
        ID3D11ShaderResourceView* getDepthTextureView() const;
    };

    class Renderer {
    private:
        const unsigned int _positionX = 0;
        const unsigned int _positionY = 0;
        Direct3D11Context *_context;
        core::dto::Resolution _resolution;
        int _count;
        core::disposing::unique_ptr<ID3D11Buffer> _indexBuffer;
        core::disposing::unique_ptr<ID3D11Buffer> _vertexBuffer;
        std::list<std::shared_ptr<direct3d11::IState>> _states;
    public:
        Renderer(Direct3D11Context *context, core::dto::Resolution resolution);
        void begin();
        void render();
        void end();
    };

    class PassSettings {
    private:
        static const core::type_slot _count = 8;
        core::TechniqueCode _code;
        ID3D11ShaderResourceView *_in[_count] = {};
        ID3D11RenderTargetView *_out[_count] = {};
        std::string _vsFunctionName;
        std::string _psFunctionName;
    public:
        PassSettings(
            core::TechniqueCode code,
            std::string vsFunctionName,
            std::string psFunctionName)
            : _code(code), _vsFunctionName(vsFunctionName), _psFunctionName(psFunctionName) {}
        void setIn(core::type_slot slot, ID3D11ShaderResourceView *view) {
            _in[slot] = view;
            log(core::stringFormat("set in: [ slot: %u, view: %p ]", slot, view));
        }
        void setOut(core::type_slot slot, ID3D11RenderTargetView *view) {
            _out[slot] = view;
            log(core::stringFormat("set out: [ slot: %u, view: %p ]", slot, view));
        }
        ID3D11ShaderResourceView* const* getIn(core::type_slot &count) const {
            count = _count;
            return _in;
        }
        ID3D11RenderTargetView* const* getOut(core::type_slot &count) const {
            count = _count;
            return _out;
        }
        const char* getVertexShaderCode() const {
            return _code.vsCode.c_str();
        }
        const char* getVertexShaderFunctionName() const {
            return _vsFunctionName.c_str();
        }
        const char* getPixelShaderCode() const {
            return _code.psCode.c_str();
        }
        const char* getPixelShaderFunctionName() const {
            return _psFunctionName.c_str();
        }
    };

    class Pass {
    private:
        Direct3D11Context *_context;
        PassSettings *_passSettings;
        core::dto::Resolution _resolution;
        dto::MatrixBufferType _matrixBufferType;
        std::unique_ptr<Renderer> _renderer;
        core::disposing::unique_ptr<ID3D11VertexShader> _vertexShader;
        core::disposing::unique_ptr<ID3D11PixelShader> _pixelShader;
        core::disposing::unique_ptr<ID3D11InputLayout> _inputLayout;
        core::disposing::unique_ptr<ID3D11Buffer> _matrixBuffer;
        core::disposing::unique_ptr<ID3D11SamplerState> _pointSampler;
        core::disposing::unique_ptr<ID3D11SamplerState> _bilinearSampler;
        std::list<std::shared_ptr<direct3d11::IState>> _states;
    public:
        Pass(Direct3D11Context *context, PassSettings *passSettings, core::dto::Resolution resolution);
        virtual ~Pass() {}
        void render();
    };
}