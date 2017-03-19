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
        virtual ID3D11ShaderResourceView* getShaderView() const override {
            return _textureShaderView;
        }
        virtual void setAsRenderer() override {
            auto deviceContext = _context->getDeviceContext();
            deviceContext->OMSetRenderTargets(1, &_textureRenderView, nullptr);
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
        virtual ID3D11ShaderResourceView* getShaderView() const override {
            return _textureShaderView.get();
        }
        virtual void setAsRenderer() override {
            auto deviceContext = _context->getDeviceContext();
            auto renderTargetView = _textureRenderView.get();
            deviceContext->OMSetRenderTargets(1, &renderTargetView, nullptr);
        }
    };

    class RenderingProxy {
    private:
        unsigned int _count = 0;
        unsigned int _index = 0;
        Direct3D11Context *_context;
        dto::RenderingResolution _resolution;
        ID3D11Texture2D *_inputColorTexture;
        ID3D11Texture2D *_inputDepthTexture;
        std::unique_ptr<CustomizableTexture> _renderingTexture;
        std::unique_ptr<ITexture> _inputTexture;
        std::unique_ptr<ITexture> _chainFirstTexture;
        std::unique_ptr<ITexture> _chainSecondTexture;
        core::disposing::unique_ptr<ID3D11Texture2D> _depthTexture;
        core::disposing::unique_ptr<ID3D11ShaderResourceView> _depthTextureView;
        core::disposing::unique_ptr<ID3D11RenderTargetView> _inputRenderTargetView;
        core::disposing::unique_ptr<ID3D11RasterizerState> _rasterizerState;
        core::disposing::unique_ptr<ID3D11DepthStencilState> _depthStencilState;
        std::list<std::shared_ptr<direct3d11::IState>> _states;
    public:
        RenderingProxy(Direct3D11Context *context, dto::RenderingResolution resolution, ID3D11Texture2D *inputColorTexture, ID3D11Texture2D *inputDepthTexture);
        virtual ~RenderingProxy() {}
        void begin();
        void end();
        void iterate();
        ITexture* nextColorTexture();
        ITexture* getRenderingDestinationTexture() const;
        ID3D11ShaderResourceView* getDepthTextureView() const;
    };

    class SquareRenderTarget {
    private:
        const unsigned int _positionX = 0;
        const unsigned int _positionY = 0;
        Direct3D11Context *_context;
        dto::RenderingResolution _resolution;
        int _count;
        core::disposing::unique_ptr<ID3D11Buffer> _indexBuffer;
        core::disposing::unique_ptr<ID3D11Buffer> _vertexBuffer;
        std::list<std::shared_ptr<direct3d11::IState>> _states;
    public:
        SquareRenderTarget(Direct3D11Context *context, dto::RenderingResolution resolution);
        void begin();
        void render();
        void end();
    };

    class ShaderCode {
    private:
        static const unsigned int _textureViewsCount = 8;
        ID3D11ShaderResourceView *_textureViews[_textureViewsCount] = {};
        std::string _vertexShaderCode;
        std::string _vertexShaderFunctionName;
        std::string _pixelShaderCode;
        std::string _pixelShaderFunctionName;
    public:
        void setColorTextureView(ID3D11ShaderResourceView *textureView) {
            _textureViews[0] = textureView;
        }
        void setDepthTextureView(ID3D11ShaderResourceView *textureView) {
            _textureViews[1] = textureView;
        }
        void setTextureView(ID3D11ShaderResourceView *textureView, unsigned int slot) {
            if (slot <= 1 || slot >= _textureViewsCount) {
                throw core::exception::OperationException(ENCRYPT_STRING("direct3d11::ShaderCode"), ENCRYPT_STRING("invalid texture view slot"));
            }

            _textureViews[slot] = textureView;
        }
        void setVertexShaderCode(std::string code, std::string functionName) {
            _vertexShaderCode = code;
            _vertexShaderFunctionName = functionName;
        }
        void setPixelShaderCode(std::string code, std::string functionName) {
            _pixelShaderCode = code;
            _pixelShaderFunctionName = functionName;
        }
        ID3D11ShaderResourceView* const* getTextureViews(unsigned int &count) const {
            count = _textureViewsCount;
            return _textureViews;
        }
        const char* getVertexShaderCode() const {
            return _vertexShaderCode.c_str();
        }
        const char* getPixelShaderCode() const {
            return _pixelShaderCode.c_str();
        }
        const char* getVertexShaderFunctionName() const {
            return _vertexShaderFunctionName.c_str();
        }
        const char* getPixelShaderFunctionName() const {
            return _pixelShaderFunctionName.c_str();
        }
    };

    class ShaderCodeFactory {
    private:
        std::string _codeDirectoryPath;
        dto::RenderingResolution _resolution;
        std::unique_ptr<core::ShaderCodeBuilder> getCodeBuilder();
    public:
        ShaderCodeFactory(std::string codeDirectoryPath, dto::RenderingResolution resolution)
            : _codeDirectoryPath(codeDirectoryPath), _resolution(resolution) { }
        ShaderCode createAlphaCode(ID3D11ShaderResourceView *colorTextureView);
        ShaderCode createDenoiseCode(ID3D11ShaderResourceView *colorTextureView, core::dto::Denoise denoise);
        ShaderCode createHDRCode(ID3D11ShaderResourceView *colorTextureView, core::dto::HDR hdr);
        ShaderCode createLiftGammaGainCode(ID3D11ShaderResourceView *colorTextureView, core::dto::LiftGammaGain liftGammaGain);
        ShaderCode createLumaSharpenCode(ID3D11ShaderResourceView *colorTextureView, core::dto::LumaSharpen lumaSharpen);
        ShaderCode createLuminescenceCode(ID3D11ShaderResourceView *colorTextureView);
        ShaderCode createTonemapCode(ID3D11ShaderResourceView *colorTextureView, core::dto::Tonemap tonemap);
        ShaderCode createVibranceCode(ID3D11ShaderResourceView *colorTextureView, core::dto::Vibrance vibrance);
        ShaderCode createBokehDoFCoCCode(ID3D11ShaderResourceView *colorTextureView, ID3D11ShaderResourceView *depthTextureView, core::dto::BokehDoF bokehDoF, core::dto::DepthBuffer depthBuffer);
        ShaderCode createBokehDoFCode(ID3D11ShaderResourceView *colorTextureView, ID3D11ShaderResourceView *depthTextureView, ID3D11ShaderResourceView *previousPassTextureView, core::BokehDoFPassType passType, core::dto::BokehDoF bokehDoF, core::dto::DepthBuffer depthBuffer);
        ShaderCode createBokehDoFChromaticAberrationCode(ID3D11ShaderResourceView *previousPassTextureView, core::dto::BokehDoF bokehDoF);
        ShaderCode createBokehDoFBlendingCode(ID3D11ShaderResourceView *colorTextureView, ID3D11ShaderResourceView *depthTextureView, ID3D11ShaderResourceView *previousPassTextureView, core::dto::BokehDoF bokehDoF, core::dto::DepthBuffer depthBuffer);
        ShaderCode createDepthRenderingCode(ID3D11ShaderResourceView *colorTextureView, ID3D11ShaderResourceView *depthTextureView, core::dto::DepthBuffer depthBuffer);
        ShaderCode createHorizontalGaussianBlurCode(ID3D11ShaderResourceView *colorTextureView, unsigned int size, float sigma);
        ShaderCode createVerticalGaussianBlurCode(ID3D11ShaderResourceView *colorTextureView, unsigned int size, float sigma);
    };

    class ShaderApplicator {
    private:
        Direct3D11Context *_context;
        ShaderCode _code;
        dto::MatrixBufferType _matrixBufferType;
        core::disposing::unique_ptr<ID3D11VertexShader> _vertexShader;
        core::disposing::unique_ptr<ID3D11PixelShader> _pixelShader;
        core::disposing::unique_ptr<ID3D11InputLayout> _inputLayout;
        core::disposing::unique_ptr<ID3D11Buffer> _matrixBuffer;
        core::disposing::unique_ptr<ID3D11SamplerState> _pointSampler;
        core::disposing::unique_ptr<ID3D11SamplerState> _bilinearSampler;
        std::list<std::shared_ptr<direct3d11::IState>> _states;
    public:
        ShaderApplicator(Direct3D11Context *context, ShaderCode code, dto::RenderingResolution resolution);
        virtual ~ShaderApplicator() {}
        void begin();
        void end();
    };
}