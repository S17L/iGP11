#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "counters.h"
#include "igp11direct3d11.h"
#include "effect.h"
#include "logger.h"
#include "texturecache.h"
#include "timeprovider.h"
#include "utility.h"

typedef HRESULT(__stdcall *IDXGIFactoryCreateSwapChainDefinition) (IDXGIFactory *dxgiFactory, IUnknown *device, DXGI_SWAP_CHAIN_DESC *description, IDXGISwapChain **chain);
HRESULT __stdcall dxgiFactoryCreateSwapChainOverride(IDXGIFactory *dxgiFactory, IUnknown *device, DXGI_SWAP_CHAIN_DESC *description, IDXGISwapChain **chain);

typedef HRESULT(__stdcall *IDXGISwapChainResizeBuffersDefinition) (IDXGISwapChain *chain, UINT bufferCount, UINT width, UINT height, DXGI_FORMAT newFormat, UINT chainFlags);
HRESULT __stdcall dxgiSwapChainResizeBuffersOverride(IDXGISwapChain *chain, UINT bufferCount, UINT width, UINT height, DXGI_FORMAT newFormat, UINT chainFlags);

typedef HRESULT(__stdcall *IDXGISwapChainResizeTargetDefinition) (IDXGISwapChain *chain, const DXGI_MODE_DESC *newDescription);
HRESULT __stdcall dxgiSwapChainResizeTargetOverride(IDXGISwapChain *chain, const DXGI_MODE_DESC *newDescription);

typedef HRESULT(__stdcall *IDXGISwapChainPresentDefinition) (IDXGISwapChain *chain, UINT interval, UINT flags);
HRESULT __stdcall dxgiSwapChainPresentOverride(IDXGISwapChain *chain, UINT interval, UINT flags);

typedef HRESULT(__stdcall *IDXGISwapChainPresent1Definition) (IDXGISwapChain1 *chain, UINT interval, UINT flags, const DXGI_PRESENT_PARAMETERS *parameters);
HRESULT __stdcall dxgiSwapChainPresent1Override(IDXGISwapChain1 *chain, UINT interval, UINT flags, const DXGI_PRESENT_PARAMETERS *parameters);

typedef HRESULT(__stdcall *ID3D11DeviceCreateTexture2DDefinition) (ID3D11Device *device, const D3D11_TEXTURE2D_DESC *description, const D3D11_SUBRESOURCE_DATA *initialData, ID3D11Texture2D **texture2D);
HRESULT __stdcall d3d11DeviceCreateTexture2DOverride(ID3D11Device *device, const D3D11_TEXTURE2D_DESC *description, const D3D11_SUBRESOURCE_DATA *initialData, ID3D11Texture2D **texture2D);

typedef HRESULT(__stdcall *ID3D11DeviceCreateShaderResourceViewDefinition) (ID3D11Device *device, ID3D11Resource *resource, const D3D11_SHADER_RESOURCE_VIEW_DESC *description, ID3D11ShaderResourceView **shaderResourceView);
HRESULT __stdcall d3d11DeviceCreateShaderResourceViewOverride(ID3D11Device *device, ID3D11Resource *resource, const D3D11_SHADER_RESOURCE_VIEW_DESC *description, ID3D11ShaderResourceView **shaderResourceView);

typedef void(__stdcall *ID3D11DeviceContextPSSetShaderResourcesDefinition) (ID3D11DeviceContext *deviceContext, UINT startSlot, UINT viewCount, ID3D11ShaderResourceView *const *shaderResourceViews);
void __stdcall d3d11DeviceContextPSSetShaderResourcesOverride(ID3D11DeviceContext *deviceContext, UINT startSlot, UINT viewCount, ID3D11ShaderResourceView *const *shaderResourceViews);

typedef void(__stdcall *ID3D11DeviceContextOMSetDepthStencilStateDefinition) (ID3D11DeviceContext *deviceContext, ID3D11DepthStencilState *depthStencilState, UINT stencil);
void __stdcall d3d11DeviceContextOMSetDepthStencilStateOverride(ID3D11DeviceContext *deviceContext, ID3D11DepthStencilState *depthStencilState, UINT stencil);

typedef void(__stdcall *ID3D11DeviceContextOMSetRenderTargetsDefinition) (ID3D11DeviceContext *deviceContext, UINT viewCount, ID3D11RenderTargetView *const *renderTargetViews, ID3D11DepthStencilView *depthStencilView);
void __stdcall d3d11DeviceContextOMSetRenderTargetsOverride(ID3D11DeviceContext *deviceContext, UINT viewCount, ID3D11RenderTargetView *const *renderTargetViews, ID3D11DepthStencilView *depthStencilView);

struct Texture {
    std::string filePath;
    ID3D11Resource *resource;
    Texture(std::string filePath, ID3D11Resource *resource) {
        this->filePath = filePath;
        this->resource = resource;
    }
};

class Direct3D11Plugin : public core::IDirect3D11Plugin, public direct3d11::IApplicator {
	friend HRESULT __stdcall dxgiFactoryCreateSwapChainOverride(IDXGIFactory *dxgiFactory, IUnknown *device, DXGI_SWAP_CHAIN_DESC *description, IDXGISwapChain **chain);
	friend HRESULT __stdcall dxgiSwapChainResizeBuffersOverride(IDXGISwapChain *chain, UINT bufferCount, UINT width, UINT height, DXGI_FORMAT newFormat, UINT chainFlags);
	friend HRESULT __stdcall dxgiSwapChainResizeTargetOverride(IDXGISwapChain *chain, const DXGI_MODE_DESC *newDescription);
	friend HRESULT __stdcall dxgiSwapChainPresentOverride(IDXGISwapChain *chain, UINT syncInterval, UINT flags);
    friend HRESULT __stdcall dxgiSwapChainPresent1Override(IDXGISwapChain1 *chain, UINT interval, UINT flags, const DXGI_PRESENT_PARAMETERS *parameters);
	friend HRESULT __stdcall d3d11DeviceCreateTexture2DOverride(ID3D11Device *device, const D3D11_TEXTURE2D_DESC *description, const D3D11_SUBRESOURCE_DATA *initialData, ID3D11Texture2D **texture2D);
	friend HRESULT __stdcall d3d11DeviceCreateShaderResourceViewOverride(ID3D11Device *device, ID3D11Resource *resource, const D3D11_SHADER_RESOURCE_VIEW_DESC *description, ID3D11ShaderResourceView **shaderResourceView);
	friend void __stdcall d3d11DeviceContextPSSetShaderResourcesOverride(ID3D11DeviceContext *deviceContext, UINT startSlot, UINT viewCount, ID3D11ShaderResourceView *const *shaderResourceViews);
    friend void __stdcall d3d11DeviceContextOMSetDepthStencilStateOverride(ID3D11DeviceContext *deviceContext, ID3D11DepthStencilState *depthStencilState, UINT stencil);
	friend void __stdcall d3d11DeviceContextOMSetRenderTargetsOverride(ID3D11DeviceContext *deviceContext, UINT viewCount, ID3D11RenderTargetView *const *renderTargetViews, ID3D11DepthStencilView *depthStencilView);
private:
	std::mutex _mutex;
	bool _initialized = false;
	core::ActivationStatus _activationStatus;
	core::IHookService *_hookService;
	core::IProcessService *_processService;
    core::ISerializer *_serializer;
    core::dto::PluginSettings _pluginSettings;
	core::dto::Direct3D11Settings _settings;
	direct3d11::IProfilePicker *_profilePicker;
	direct3d11::ITextureService *_textureService;
    std::list<Texture> _textures;
	std::unique_ptr<direct3d11::IProfile> _profile;
	std::unique_ptr<core::IFrameCounter> _frameCounter;
	std::shared_ptr<core::ITextureCache> _textureCache;
	std::unique_ptr<core::time::ITimeProvider> _timeProvider;
	std::unique_ptr<direct3d11::Direct3D11Context> _context;
	std::unique_ptr<direct3d11::EffectsApplicator> _applicator;
	IDXGIFactoryCreateSwapChainDefinition _dxgiFactoryCreateSwapChain = nullptr;
	IDXGISwapChainResizeBuffersDefinition _dxgiSwapChainResizeBuffers = nullptr;
	IDXGISwapChainResizeTargetDefinition _dxgiSwapChainResizeTarget = nullptr;
	IDXGISwapChainPresentDefinition _dxgiSwapChainPresent = nullptr;
    IDXGISwapChainPresent1Definition _dxgiSwapChainPresent1 = nullptr;
	ID3D11DeviceCreateTexture2DDefinition _d3d11DeviceCreateTexture2D = nullptr;
	ID3D11DeviceCreateShaderResourceViewDefinition _d3d11DeviceCreateShaderResourceView = nullptr;
	ID3D11DeviceContextPSSetShaderResourcesDefinition _d3d11DeviceContextPSSetShaderResources = nullptr;
    ID3D11DeviceContextOMSetDepthStencilStateDefinition _d3d11DeviceContextOMSetDepthStencilState = nullptr;
	ID3D11DeviceContextOMSetRenderTargetsDefinition _d3d11DeviceContextSetRenderTargets = nullptr;
	Direct3D11Plugin()
		: _activationStatus(core::ActivationStatus::pluginloaded) {};
    void createProfile();
    void deinitializeApplicator();
	void initialize(IDXGISwapChain *chain);
	void printPerformanceStatistics();
    direct3d11::dto::FilterSettings getFilterConfiguration();
public:
	static Direct3D11Plugin& getInstance() {
		static Direct3D11Plugin instance;
		return instance;
	}
	~Direct3D11Plugin();
	Direct3D11Plugin(Direct3D11Plugin const&) = delete;
	void operator=(Direct3D11Plugin const&) = delete;
	bool isInitialized();
	bool initialize(
		core::IHookService *hookService,
		core::IProcessService *processService,
        core::ISerializer *serializer,
		core::ITextureCacheFactory *textureCacheFactory,
        core::dto::PluginSettings pluginSettings,
		core::dto::Direct3D11Settings settings,
		direct3d11::IProfilePicker *profilePicker,
		direct3d11::ITextureService *textureService);
	bool deinitialize();
	bool start();
	bool stop();
	virtual core::ActivationStatus getActivationStatus() override {
		return _activationStatus;
	}
    virtual core::dto::Direct3D11Settings getSettings() override {
        return _settings;
    }
	virtual bool update(core::dto::Direct3D11Settings settings) override;
	virtual void applyPostProcessing(const direct3d11::dto::PostProcessingSettings &postProcessingSettings) override;
    virtual bool initializationRequired(const direct3d11::dto::PostProcessingSettings &postProcessingSettings) override;
};