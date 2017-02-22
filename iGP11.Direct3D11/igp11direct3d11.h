#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace direct3d11 {
    struct ProfileConfiguration;
    class Direct3D11Context;
    class IApplicator;
    class IEffect;
    class IProfile;
    class IProfilePicker;
    class ITexture;
    class ITextureService;

    namespace dto {
        struct GaussianBlurConfiguration {
            unsigned int size = 0;
            float sigma = 0;
            GaussianBlurConfiguration() {}
            GaussianBlurConfiguration(unsigned int size, float sigma) {
                this->size = size;
                this->sigma = sigma;
            }
        };

        struct PostProcessingConfiguration {
            ID3D11Texture2D *colorTexture;
            ID3D11Texture2D *depthTexture;
        };

        struct RenderingResolution {
            unsigned int height;
            unsigned int width;
        };
    }

    struct ProfileConfiguration {
        std::string dumpingDirectory;
        Direct3D11Context *context;
        IApplicator *applicator;
        ITextureService *textureService;
    };

    class Direct3D11Context {
    private:
        IDXGISwapChain *_chain;
    public:
        Direct3D11Context(IDXGISwapChain *chain)
            : _chain(chain) {}
        IDXGISwapChain* getChain();
        core::disposing::unique_ptr<ID3D11Device> getDevice();
        core::disposing::unique_ptr<ID3D11DeviceContext> getDeviceContext();
    };

    class IApplicator {
    public:
        virtual ~IApplicator() {}
        virtual void applyPostProcessing(const dto::PostProcessingConfiguration &configuration) = 0;
        virtual bool initializationRequired(const dto::PostProcessingConfiguration &configuration) = 0;
    };

    class IEffect {
    public:
        virtual ~IEffect() {}
        virtual std::string getName() = 0;
        virtual void begin() = 0;
        virtual void render() = 0;
        virtual void end() = 0;
    };

    class IProfile {
    public:
        virtual ~IProfile() {}
        virtual void outputMergerSetDepthStencilState(ID3D11DepthStencilState *depthStencilState, UINT stencil) = 0;
        virtual void outputMergerSetRenderTargets(UINT viewCount, ID3D11RenderTargetView * const *renderTargetViews, ID3D11DepthStencilView *depthStencilView) = 0;
        virtual void pixelShaderSetShaderResources(UINT startSlot, UINT viewCount, ID3D11ShaderResourceView *const *shaderResourceViews) = 0;
        virtual void presentFrame() = 0;
    };

    class IProfilePicker {
    public:
        virtual ~IProfilePicker() {}
        virtual std::unique_ptr<IProfile> getProfile(core::Direct3D11ProfileType type, ProfileConfiguration configuration) = 0;
    };

    class IState {
    public:
        virtual ~IState() {}
        virtual void acquire() = 0;
        virtual void restore() = 0;
    };

    class ITexture {
    public:
        virtual ~ITexture() {}
        virtual ID3D11Texture2D* get() const = 0;
        virtual ID3D11ShaderResourceView* getShaderView() const = 0;
        virtual void setAsRenderer() = 0;
    };

    class ITextureService {
    public:
        virtual ~ITextureService() {}
        virtual HRESULT saveTextureToFile(direct3d11::Direct3D11Context *context, ID3D11Resource *texture, std::string filePath) = 0;
        virtual HRESULT createTextureFromFile(direct3d11::Direct3D11Context *context, ID3D11Resource **texture, ID3D11ShaderResourceView **textureView, std::string filePath, bool forceSrgb) = 0;
    };
}