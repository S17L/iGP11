#include "stdafx.h"
#include "direct3d11plugin.h"

using namespace core::logging;

class ResourceDetail {
public:
    std::string id;
    ID3D11Resource *resource;
    D3D11_SHADER_RESOURCE_VIEW_DESC description;
};

__declspec(thread) bool _hookingEnabled = true;
__declspec(thread) ResourceDetail _resourceDetail;

class DisabledHookingScope {
public:
    DisabledHookingScope() {
        _hookingEnabled = false;
    }
    ~DisabledHookingScope() {
        _hookingEnabled = true;
    }
};

void setResourceDetail(std::string id, ID3D11Resource *resource, D3D11_SHADER_RESOURCE_VIEW_DESC description) {
    _resourceDetail.id = id;
    _resourceDetail.resource = resource;
    _resourceDetail.description = description;
}

void clearResourceDetail() {
    _resourceDetail.id.clear();
    _resourceDetail.resource = nullptr;
    ZeroMemory(&_resourceDetail.description, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
}

D3D11_SHADER_RESOURCE_VIEW_DESC applyTextureDetailLevel(D3D11_SHADER_RESOURCE_VIEW_DESC description, core::TextureDetailLevel detailLevel) {
    int level;
    switch (detailLevel) {
    case core::TextureDetailLevel::lowest:
        level = 1;
        break;
    case core::TextureDetailLevel::low:
        level = 4;
        break;
    case core::TextureDetailLevel::medium:
        level = 7;
        break;
    case core::TextureDetailLevel::high:
        level = 10;
        break;
    default:
        return description;
    }

    const unsigned int minLevel = 1;
    const unsigned int maxLevel = 16;

    if (description.Texture2D.MipLevels > minLevel && description.Texture2D.MipLevels < maxLevel) {
        description.Texture2D.MostDetailedMip = max((int)description.Texture2D.MipLevels - level, 0);
        description.Texture2D.MipLevels = -1;
    }

    return description;
}

direct3d11::dto::FilterConfiguration getFilterConfiguration(core::dto::Direct3D11Settings settings) {
    direct3d11::dto::FilterConfiguration configuration;
    configuration.bokehDoF = settings.bokehDoF;
    configuration.depthBuffer = settings.depthBuffer;
    configuration.pluginSettings = settings.pluginSettings;
    configuration.lumaSharpen = settings.lumaSharpen;
    configuration.tonemap = settings.tonemap;
    configuration.vibrance = settings.vibrance;

    return configuration;
}

LRESULT CALLBACK windowProcedure(HWND handle, UINT message, WPARAM handles, LPARAM pointers) {
    return DefWindowProc(handle, message, handles, pointers);
}

void Direct3D11Plugin::createProfile() {
    direct3d11::ProfileConfiguration configuration;
    configuration.applicator = this;
    configuration.context = _context.get();
    configuration.dumpingDirectory = _settings.textures.dumpingPath;
    configuration.textureService = _textureService;

    _profile.reset();
    _profile = std::move(_profilePicker->getProfile(_settings.pluginSettings.profileType, configuration));
}

void Direct3D11Plugin::deinitializeApplicator() {
    if (_applicator != nullptr) {
        ThreadLoggerAppenderScope scope(debug, ENCRYPT_STRING("deinitializing applicator"));
        _applicator->deinitialize();
        _applicator.reset();
    }
}

void Direct3D11Plugin::initialize(IDXGISwapChain *chain) {
    _activationStatus = core::ActivationStatus::pluginactivated;
    _context.reset(new direct3d11::Direct3D11Context(chain));

    createProfile();
}

void Direct3D11Plugin::printPerformanceStatistics() {
    auto description = direct3d11::utility::getRenderingResolution(_context->getChain());
    auto message = core::stringFormat(
        ENCRYPT_STRING("FPS: %u, render: [ width: %u, height: %u ]"),
        _frameCounter->getAverageCount(),
        description.width,
        description.height);

    log(message);
}

Direct3D11Plugin::~Direct3D11Plugin() {
    log(ENCRYPT_STRING("direct3d11 unitialized"));
}

HRESULT __stdcall dxgiFactoryCreateSwapChainOverride(IDXGIFactory *dxgiFactory, IUnknown *device, DXGI_SWAP_CHAIN_DESC *description, IDXGISwapChain **swapChain) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    ThreadLoggerAppenderScope scope(debug, ENCRYPT_STRING("creating swap chain"));
    log(debug, core::stringFormat(ENCRYPT_STRING("[ description: [ %s ]"), direct3d11::stringify::toString(description).c_str()));

    _this.deinitializeApplicator();

    auto result = _this._dxgiFactoryCreateSwapChain(dxgiFactory, device, description, swapChain);
    log(debug, ENCRYPT_STRING("call"), result);

    if (SUCCEEDED(result) && _this._activationStatus == core::ActivationStatus::pluginactivated) {
        log(debug, ENCRYPT_STRING("reinitialization requested"));
        _this._activationStatus = core::ActivationStatus::pluginactivationpending;
    }

    return result;
}

HRESULT __stdcall dxgiSwapChainResizeBuffersOverride(IDXGISwapChain *swapChain, UINT bufferCount, UINT width, UINT height, DXGI_FORMAT newFormat, UINT flags) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    ThreadLoggerAppenderScope scope(debug, ENCRYPT_STRING("resizing swap chain buffers"));
    log(debug, core::stringFormat(ENCRYPT_STRING("[ count: %u, %ux%u, format: %u, flags: %u ]"), bufferCount, width, height, newFormat, flags));

    _this.deinitializeApplicator();

    auto result = _this._dxgiSwapChainResizeBuffers(swapChain, bufferCount, width, height, newFormat, flags);
    log(debug, ENCRYPT_STRING("call"), result);

    if (SUCCEEDED(result) && _this._activationStatus == core::ActivationStatus::pluginactivated) {
        log(debug, ENCRYPT_STRING("reinitialization requested"));
        _this._activationStatus = core::ActivationStatus::pluginactivationpending;
    }

    return result;
}

HRESULT __stdcall dxgiSwapChainResizeTargetOverride(IDXGISwapChain *swapChain, const DXGI_MODE_DESC *description) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    ThreadLoggerAppenderScope scope(debug, ENCRYPT_STRING("resizing swap chain target"));
    log(core::stringFormat(ENCRYPT_STRING("call: [ description: %s ]"), direct3d11::stringify::toString(description).c_str()));

    _this.deinitializeApplicator();

    auto result = _this._dxgiSwapChainResizeTarget(swapChain, description);
    log(debug, ENCRYPT_STRING("call"), result);

    if (SUCCEEDED(result) && _this._activationStatus == core::ActivationStatus::pluginactivated) {
        log(debug, ENCRYPT_STRING("reinitialization requested"));
        _this._activationStatus = core::ActivationStatus::pluginactivationpending;
    }

    return result;
}

HRESULT __stdcall dxgiSwapChainPresentOverride(IDXGISwapChain *chain, UINT interval, UINT flags) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    if (_hookingEnabled) {
        if (_this._activationStatus == core::ActivationStatus::pluginactivationpending) {
            log(ENCRYPT_STRING("activation pending... initializing"));
            _this.initialize(chain);
        }
        else if (_this._activationStatus == core::ActivationStatus::pluginactivated && _this._context->getChain() != chain) {
            log(ENCRYPT_STRING("swap chain changed... reinitializing"));
            _this.initialize(chain);
        }

        if (_this._activationStatus == core::ActivationStatus::pluginactivated) {
            if (_this._applicator == nullptr) {
                _this._applicator.reset(new direct3d11::EffectsApplicator(::getFilterConfiguration(_this._settings), _this._context.get()));
            }

            if (_this._frameCounter->nextFrame()) {
                _this.printPerformanceStatistics();
            }

            _this._profile->presentFrame();
        }
    }

    return _this._dxgiSwapChainPresent(chain, interval, flags);
}

HRESULT __stdcall dxgiSwapChainPresent1Override(IDXGISwapChain1 *chain, UINT interval, UINT flags, const DXGI_PRESENT_PARAMETERS *parameters) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();
    return _this._dxgiSwapChainPresent1(chain, interval, flags, parameters);
}

HRESULT __stdcall d3d11DeviceCreateTexture2DOverride(ID3D11Device *device, const D3D11_TEXTURE2D_DESC *description, const D3D11_SUBRESOURCE_DATA *initialData, ID3D11Texture2D **texture2D) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    if (_hookingEnabled
        && _this._activationStatus == core::ActivationStatus::pluginactivated
        && description != nullptr
        && description->ArraySize == 1
        && description->BindFlags & D3D11_BIND_SHADER_RESOURCE
        && initialData != nullptr
        && initialData->pSysMem != nullptr
        && initialData->SysMemSlicePitch > 0) {

        auto textureOverrideMode = _this._settings.textures.overrideMode;
        if (textureOverrideMode == core::TextureOverrideMode::dumping || textureOverrideMode == core::TextureOverrideMode::override) {
            const unsigned int resolution = 417;
            const unsigned int seed = 17;
            uint64_t hash = core::algorithm::hash64(initialData->pSysMem, initialData->SysMemSlicePitch, resolution, seed);

            if (hash > 0) {
                auto textureId = core::stringFormat(ENCRYPT_STRING("%lu_%llu"), initialData->SysMemSlicePitch, hash);
                if (textureOverrideMode == core::TextureOverrideMode::dumping) {
                    auto filePath = core::file::combine(_this._settings.textures.dumpingPath, core::stringFormat(ENCRYPT_STRING("%s.dds"), textureId.c_str()));
                    auto result = _this._d3d11DeviceCreateTexture2D(device, description, initialData, texture2D);
                    if (FAILED(result)) {
                        log(error, core::stringFormat(ENCRYPT_STRING("texture2d creation failed [ path: %s, result: 0x%08lx ]"), filePath.c_str(), result));
                    }
                    else if (!core::file::fileExists(filePath)) {
                        std::lock_guard<std::mutex> lock(_this._mutex);
                        
                        HRESULT dumpingResult = S_FALSE;
                        log(core::stringFormat(ENCRYPT_STRING("attempting to dump texture2d: [ path: %s ]"), filePath.c_str()));

                        {
                            DisabledHookingScope scope;
                            dumpingResult = _this._textureService->saveTextureToFile(
                                _this._context.get(),
                                reinterpret_cast<ID3D11Resource*>(*texture2D),
                                filePath);
                        }

                        if (SUCCEEDED(dumpingResult)) {
                            log(core::stringFormat(ENCRYPT_STRING("texture2d dumped: [ path: %s ]"), filePath.c_str()));
                        }
                        else {
                            log(error, core::stringFormat(ENCRYPT_STRING("texture2d dumping failed: [ path: %s, result: 0x%08lx ]"), filePath.c_str(), dumpingResult));
                        }
                    }

                    return result;
                }
                else if (textureOverrideMode == core::TextureOverrideMode::override && _this._textureCache->has(textureId)) {
                    core::TextureProfile profile;
                    profile.mapFrom = textureId;
                    profile.mapTo = textureId;
                    _this._textureCache->merge(profile);

                    HRESULT result = S_FALSE;
                    ID3D11Texture2D *oldValue = *texture2D;
                    ID3D11ShaderResourceView *textureView;
                    auto filePath = core::file::combine(_this._settings.textures.overridePath, core::stringFormat(ENCRYPT_STRING("%s.dds"), profile.mapTo.get().c_str()));

                    {
                        DisabledHookingScope scope;
                        result = _this._textureService->createTextureFromFile(
                            _this._context.get(),
                            reinterpret_cast<ID3D11Resource**>(texture2D),
                            &textureView,
                            filePath,
                            profile.forceSrgb.get());
                    }

                    if (FAILED(result)) {
                        log(error, core::stringFormat(ENCRYPT_STRING("texture2d creation failed [ path: %s, result: 0x%08lx ]"), filePath.c_str(), result));
                        *texture2D = oldValue;
                    }
                    else if (textureView != nullptr) {
                        auto textureViewComponent = core::disposing::makeUnknown<ID3D11ShaderResourceView>(textureView);
                        D3D11_SHADER_RESOURCE_VIEW_DESC shaderResourceViewDescription;
                        textureViewComponent->GetDesc(&shaderResourceViewDescription);
                        ::setResourceDetail(textureId, reinterpret_cast<ID3D11Resource*>(*texture2D), shaderResourceViewDescription);

                        return result;
                    }
                }
            }
        }
    }

    return _this._d3d11DeviceCreateTexture2D(device, description, initialData, texture2D);
}

HRESULT __stdcall d3d11DeviceCreateShaderResourceViewOverride(ID3D11Device *device, ID3D11Resource *resource, const D3D11_SHADER_RESOURCE_VIEW_DESC *description, ID3D11ShaderResourceView **shaderResourceView) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    auto textureOverrideMode = _this._settings.textures.overrideMode;
    if (_hookingEnabled && _this._activationStatus == core::ActivationStatus::pluginactivated) {
        if (textureOverrideMode == core::TextureOverrideMode::override && resource != nullptr && _resourceDetail.resource == resource) {
            auto overridenDescription = applyTextureDetailLevel(_resourceDetail.description, _this._settings.textures.detailLevel);
            auto result = _this._d3d11DeviceCreateShaderResourceView(device, resource, &overridenDescription, shaderResourceView);
            if (FAILED(result)) {
                log(error, core::stringFormat(ENCRYPT_STRING("texture2d resource view creation failed [ mode: %u, resource: %p, result: 0x%08lx ]"), textureOverrideMode, resource, result));
            }
            else {
                log(core::stringFormat(ENCRYPT_STRING("texture2d replaced [ id: %s, resource: %p ]"), _resourceDetail.id.c_str(), resource));
            }

            ::clearResourceDetail();
            return result;
        }

        if (description != nullptr) {
            auto overridenDescription = applyTextureDetailLevel(*description, _this._settings.textures.detailLevel);
            auto result = _this._d3d11DeviceCreateShaderResourceView(device, resource, &overridenDescription, shaderResourceView);

            if (SUCCEEDED(result)) {
                return result;
            }
        }
    }

    return _this._d3d11DeviceCreateShaderResourceView(device, resource, description, shaderResourceView);
}

void __stdcall d3d11DeviceContextPSSetShaderResourcesOverride(ID3D11DeviceContext *deviceContext, UINT startSlot, UINT viewCount, ID3D11ShaderResourceView *const *shaderResourceViews) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    if (_hookingEnabled && _this._activationStatus == core::ActivationStatus::pluginactivated) {
        _this._profile->pixelShaderSetShaderResources(startSlot, viewCount, shaderResourceViews);
    }

    _this._d3d11DeviceContextPSSetShaderResources(deviceContext, startSlot, viewCount, shaderResourceViews);
}

void __stdcall d3d11DeviceContextOMSetDepthStencilStateOverride(ID3D11DeviceContext *deviceContext, ID3D11DepthStencilState *depthStencilState, UINT stencil) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    if (_hookingEnabled && _this._activationStatus == core::ActivationStatus::pluginactivated) {
        _this._profile->outputMergerSetDepthStencilState(depthStencilState, stencil);
    }

    _this._d3d11DeviceContextOMSetDepthStencilState(deviceContext, depthStencilState, stencil);
}

void __stdcall d3d11DeviceContextOMSetRenderTargetsOverride(ID3D11DeviceContext *deviceContext, UINT viewCount, ID3D11RenderTargetView * const *renderTargetViews, ID3D11DepthStencilView *depthStencilView) {
    Direct3D11Plugin &_this = Direct3D11Plugin::getInstance();

    if (_hookingEnabled && _this._activationStatus == core::ActivationStatus::pluginactivated) {
        _this._profile->outputMergerSetRenderTargets(viewCount, renderTargetViews, depthStencilView);
    }

    _this._d3d11DeviceContextSetRenderTargets(deviceContext, viewCount, renderTargetViews, depthStencilView);
}

bool Direct3D11Plugin::isInitialized() {
    return _initialized;
}

bool Direct3D11Plugin::initialize(
    core::IHookService *hookService,
    core::IProcessService *processService,
    core::ITextureCacheFactory *textureCacheFactory,
    core::dto::Direct3D11Settings settings,
    direct3d11::IProfilePicker *profilePicker,
    direct3d11::ITextureService *textureService) {
    if (_initialized) {
        log(error, core::stringFormat(ENCRYPT_STRING("plugin is already initialized [ status: %d ]"), _activationStatus));
        return true;
    }
    else if (hookService == nullptr || processService == nullptr || textureCacheFactory == nullptr || profilePicker == nullptr || textureService == nullptr) {
        log(error, ENCRYPT_STRING("at least one provided external service is undefined"));
        return false;
    }

    _initialized = true;
    _textureService = textureService;
    _hookService = hookService;
    _processService = processService;
    _settings = settings;
    _profilePicker = profilePicker;

    auto deviceComponent = core::disposing::makeUnknown<ID3D11Device>(nullptr);
    auto deviceContextComponent = core::disposing::makeUnknown<ID3D11DeviceContext>(nullptr);
    auto chainComponent = core::disposing::makeUnknown<IDXGISwapChain>(nullptr);

    ThreadLoggerAppenderScope scope(ENCRYPT_STRING("initialization"));
    core::dto::ProcessDetail processDetail = _processService->getCurrentProcessDetail();
    log(core::stringFormat(ENCRYPT_STRING("current process [ id: %lu, path: %s ]"), processDetail.id, processDetail.path.c_str()));

    core::StateTextureCacheVisitor textureCacheVisitor;
    log(ENCRYPT_STRING("attempting to build texture cache..."));
    _textureCache = textureCacheFactory->createFromDirectory(_settings.textures.overridePath);
    _textureCache->accept(textureCacheVisitor);
    log(core::stringFormat(ENCRYPT_STRING("texture cache built, count: %lu, structure: %s"), _textureCache->getCount(), textureCacheVisitor.build().c_str()));

    _timeProvider = std::unique_ptr<core::time::ITimeProvider>(new core::time::CurrentTimeProvider());
    _frameCounter = std::unique_ptr<core::IFrameCounter>(new core::FrameCounter(_timeProvider.get()));

    std::wstring windowTitle(L"iGP11.direct3d11");
    WNDCLASSEX windowDetail;
    windowDetail.style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC;
    windowDetail.lpfnWndProc = windowProcedure;
    windowDetail.cbClsExtra = 0;
    windowDetail.cbWndExtra = 0;
    windowDetail.hInstance = GetModuleHandle(NULL);
    windowDetail.hIcon = LoadIcon(NULL, IDI_WINLOGO);
    windowDetail.hIconSm = windowDetail.hIcon;
    windowDetail.hCursor = LoadCursor(NULL, IDC_ARROW);
    windowDetail.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    windowDetail.lpszMenuName = NULL;
    windowDetail.lpszClassName = windowTitle.c_str();
    windowDetail.cbSize = sizeof(WNDCLASSEX);

    if (!RegisterClassEx(&windowDetail)) {
        log(error, core::stringFormat(ENCRYPT_STRING("window initialization failed")));
        return false;
    }

    HWND windowHandle = CreateWindowEx(WS_EX_APPWINDOW, windowTitle.c_str(), windowTitle.c_str(), WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_POPUP, 64, 64, 256, 256, NULL, NULL, windowDetail.hInstance, NULL);
    if (!windowHandle) {
        log(error, core::stringFormat(ENCRYPT_STRING("window handle not acquired")));
        return false;
    }

    log(debug, core::stringFormat(ENCRYPT_STRING("window handle acquired")));
    const int maxRetryCount = 32;
    const int retryDelay = 250;
    int retryCount = 0;
    HRESULT result = S_FALSE;

    do {
        if (retryCount > 0) {
            log(core::stringFormat(ENCRYPT_STRING("retry: %d/%d"), retryCount, maxRetryCount));
            ::Sleep(retryDelay);
        }

        retryCount++;
        DXGI_SWAP_CHAIN_DESC chainDescription;
        ZeroMemory(&chainDescription, sizeof(chainDescription));
        chainDescription.BufferDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
        chainDescription.SampleDesc.Count = 1;
        chainDescription.SampleDesc.Quality = 0;
        chainDescription.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        chainDescription.BufferCount = 1;
        chainDescription.OutputWindow = windowHandle;
        chainDescription.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
        chainDescription.Windowed = TRUE;

        ID3D11Device *device = nullptr;
        ID3D11DeviceContext *deviceContext = nullptr;
        IDXGISwapChain *chain = nullptr;

        log(ENCRYPT_STRING("attempting to initialize direct3d11..."));
        result = ::D3D11CreateDeviceAndSwapChain(
            NULL,
            D3D_DRIVER_TYPE_HARDWARE,
            NULL,
            NULL,
            NULL,
            0,
            D3D11_SDK_VERSION,
            &chainDescription,
            &chain,
            &device,
            NULL,
            &deviceContext);

        if (FAILED(result)) {
            log(core::stringFormat(ENCRYPT_STRING("direct3d11 initialization failed with error code: 0x%08lx, retrying..."), result));
        }
        else {
            log(ENCRYPT_STRING("components initialized..."));
            deviceComponent.reset(device);
            deviceContextComponent.reset(deviceContext);
            chainComponent.reset(chain);

            break;
        }
    } while (FAILED(result) && retryCount < maxRetryCount);

    if (FAILED(result) || chainComponent == nullptr) {
        log(error, ENCRYPT_STRING("direct3d11 initialization failed"));
        _activationStatus = core::ActivationStatus::pluginactivationfailed;
    }
    else {
        auto transaction = _hookService->openTransaction();
        ThreadLoggerAppenderScope scope(ENCRYPT_STRING("installing hooks"));

        auto dxgiDeviceComponent = core::disposing::makeUnknown<IDXGIDevice>(nullptr);
        auto dxgiAdapterComponent = core::disposing::makeUnknown<IDXGIAdapter>(nullptr);
        auto dxgiFactoryComponent = core::disposing::makeUnknown<IDXGIFactory>(nullptr);

        IDXGIDevice *dxgiDevicePointer = nullptr;
        result = deviceComponent->QueryInterface(__uuidof(IDXGIDevice), (void **)&dxgiDevicePointer);
        if (SUCCEEDED(result)) {
            dxgiDeviceComponent.reset(dxgiDevicePointer);
            IDXGIAdapter *dxgiAdapterPointer = nullptr;
            result = dxgiDevicePointer->GetAdapter(&dxgiAdapterPointer);
            if (SUCCEEDED(result)) {
                dxgiAdapterComponent.reset(dxgiAdapterPointer);
                IDXGIFactory *dxgiFactoryPointer = nullptr;
                result = dxgiAdapterPointer->GetParent(__uuidof(IDXGIFactory), (void **)&dxgiFactoryPointer);
                if (SUCCEEDED(result)) {
                    dxgiFactoryComponent.reset(dxgiFactoryPointer);
                    DWORD_PTR *dxgiFactoryVtable = (DWORD_PTR*)dxgiFactoryPointer;
                    dxgiFactoryVtable = (DWORD_PTR*)dxgiFactoryVtable[0];
                    transaction->hook(ENCRYPT_STRING("IDXGIFactory->CreateSwapChain"), reinterpret_cast<LPVOID>(dxgiFactoryVtable[10]), &dxgiFactoryCreateSwapChainOverride, reinterpret_cast<LPVOID*>(&_dxgiFactoryCreateSwapChain));
                }
                else {
                    log(error, core::stringFormat(ENCRYPT_STRING("IDXGIAdapter->GetParent failed with error code: 0x%08lx"), result));
                }
            }
            else {
                log(error, core::stringFormat(ENCRYPT_STRING("IDXGIDevice->GetAdapter failed with error code: 0x%08lx"), result));
            }
        }
        else {
            log(error, core::stringFormat(ENCRYPT_STRING("IDXGIDevice->QueryInterface failed with error code: 0x%08lx"), result));
        }

        /* direct3d11.1 */
        /*
        auto dxgiDevice1Component = core::disposing::makeUnknown<ID3D11Device1>(nullptr);
        auto dxgiFactory2Component = core::disposing::makeUnknown<IDXGIFactory2>(nullptr);

        if (SUCCEEDED(result)) {
            ID3D11Device1 *device1 = nullptr;
            result = deviceComponent->QueryInterface(__uuidof (ID3D11Device1), (void **)&device1);
            if (SUCCEEDED(result)) {
                dxgiDevice1Component.reset(device1);
                IDXGIFactory2 *dxgiFactory2 = nullptr;
                result = dxgiAdapterComponent->GetParent(__uuidof(IDXGIFactory2), (void **)&dxgiFactory2);
                if (SUCCEEDED(result)) {
                    dxgiFactory2Component.reset(dxgiFactory2);
                    IDXGISwapChain1 *chain1 = nullptr;
                    DXGI_SWAP_CHAIN_DESC1 swapChainDescription1 = { 0 };
                    swapChainDescription1.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
                    swapChainDescription1.BufferCount = 2;
                    swapChainDescription1.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
                    swapChainDescription1.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
                    swapChainDescription1.SampleDesc.Count = 1;
                    result = dxgiFactory2Component->CreateSwapChainForHwnd(
                        dxgiDevice1Component.get(),
                        windowHandle,
                        &swapChainDescription1,
                        NULL,
                        NULL,
                        &chain1);

                    if (SUCCEEDED(result)) {
                        DWORD_PTR *chain1Vtable = (DWORD_PTR*)chain1;
                        chain1Vtable = (DWORD_PTR*)chain1Vtable[0];
                        transaction->hook(ENCRYPT_STRING("IDXGISwapChain1->Present1"), reinterpret_cast<LPVOID>(chain1Vtable[22]), &dxgiSwapChainPresent1Override, reinterpret_cast<LPVOID*>(&_dxgiSwapChainPresent1));
                    }
                    else {
                        log(error, core::stringFormat(ENCRYPT_STRING("IDXGIFactory2->CreateSwapChainForHwnd failed with error code: 0x%08lx"), result));
                    }
                }
                else {
                    log(error, core::stringFormat(ENCRYPT_STRING("IDXGIAdapter->GetParent failed with error code: 0x%08lx"), result));
                }
            }
            else {
                log(error, core::stringFormat(ENCRYPT_STRING("ID3D11Device->QueryInterface failed with error code: 0x%08lx"), result));
            }
        }*/

        auto chainVtable = (DWORD_PTR*)chainComponent.get();
        chainVtable = (DWORD_PTR*)chainVtable[0];

        transaction->hook(ENCRYPT_STRING("IDXGISwapChain->ResizeBuffers"), reinterpret_cast<LPVOID>(chainVtable[13]), &dxgiSwapChainResizeBuffersOverride, reinterpret_cast<LPVOID*>(&_dxgiSwapChainResizeBuffers));
        transaction->hook(ENCRYPT_STRING("IDXGISwapChain->ResizeTarget"), reinterpret_cast<LPVOID>(chainVtable[14]), &dxgiSwapChainResizeTargetOverride, reinterpret_cast<LPVOID*>(&_dxgiSwapChainResizeTarget));
        transaction->hook(ENCRYPT_STRING("IDXGISwapChain->Present"), reinterpret_cast<LPVOID>(chainVtable[8]), &dxgiSwapChainPresentOverride, reinterpret_cast<LPVOID*>(&_dxgiSwapChainPresent));

        auto deviceVtable = (DWORD_PTR*)deviceComponent.get();
        deviceVtable = (DWORD_PTR*)deviceVtable[0];

        transaction->hook(ENCRYPT_STRING("ID3D11Device->CreateTexture2D"), reinterpret_cast<LPVOID>(deviceVtable[5]), &d3d11DeviceCreateTexture2DOverride, reinterpret_cast<LPVOID*>(&_d3d11DeviceCreateTexture2D));
        transaction->hook(ENCRYPT_STRING("ID3D11Device->CreateShaderResourceView"), reinterpret_cast<LPVOID>(deviceVtable[7]), &d3d11DeviceCreateShaderResourceViewOverride, reinterpret_cast<LPVOID*>(&_d3d11DeviceCreateShaderResourceView));

        auto deviceContextVtable = (DWORD_PTR*)deviceContextComponent.get();
        deviceContextVtable = (DWORD_PTR*)deviceContextVtable[0];
        transaction->hook(ENCRYPT_STRING("ID3D11DeviceContext->PSSetShaderResources"), reinterpret_cast<LPVOID>(deviceContextVtable[8]), &d3d11DeviceContextPSSetShaderResourcesOverride, reinterpret_cast<LPVOID*>(&_d3d11DeviceContextPSSetShaderResources));
        transaction->hook(ENCRYPT_STRING("ID3D11DeviceContext->OMDepthStencilState"), reinterpret_cast<LPVOID>(deviceContextVtable[36]), &d3d11DeviceContextOMSetDepthStencilStateOverride, reinterpret_cast<LPVOID*>(&_d3d11DeviceContextOMSetDepthStencilState));
        transaction->hook(ENCRYPT_STRING("ID3D11DeviceContext->OMSetRenderTargets"), reinterpret_cast<LPVOID>(deviceContextVtable[33]), &d3d11DeviceContextOMSetRenderTargetsOverride, reinterpret_cast<LPVOID*>(&_d3d11DeviceContextSetRenderTargets));

        if (SUCCEEDED(result)) {
            ThreadLoggerAppenderScope scope(ENCRYPT_STRING("committing hooks"));
            if (transaction->commit()) {
                log(information, ENCRYPT_STRING("call"), true);
                _activationStatus = core::ActivationStatus::pluginactivationpending;
            }
            else {
                log(error, ENCRYPT_STRING("call"), false);
                _activationStatus = core::ActivationStatus::pluginactivationfailed;
            }
        }
        else {
            _activationStatus = core::ActivationStatus::pluginactivationfailed;
        }

        if (_activationStatus != core::ActivationStatus::pluginactivationfailed) {
            log(ENCRYPT_STRING("direct3d11 initialization done, awaiting first invocation..."));
        }
        else {
            log(error, ENCRYPT_STRING("direct3d11 initialization failed due to hooking issues"));
        }
    }

    {
        ThreadLoggerAppenderScope scope(ENCRYPT_STRING("cleaning up temporary data"));
        ::DestroyWindow(windowHandle);
    }

    return _activationStatus != core::ActivationStatus::pluginactivationfailed;
}

bool Direct3D11Plugin::deinitialize() {
    return true;
}

bool Direct3D11Plugin::start() {
    return true;
}

bool Direct3D11Plugin::stop() {
    return true;
}

bool Direct3D11Plugin::update(core::dto::Direct3D11Settings settings) {
    if (_applicator != nullptr) {
        _settings = settings;
        _applicator->update(::getFilterConfiguration(_settings));

        return true;
    }

    return false;
}

void Direct3D11Plugin::applyPostProcessing(const direct3d11::dto::PostProcessingConfiguration &configuration) {
    if (_applicator != nullptr) {
        DisabledHookingScope scope;
        _applicator->apply(configuration);
    }
}

bool Direct3D11Plugin::initializationRequired(const direct3d11::dto::PostProcessingConfiguration &configuration) {
    return _applicator != nullptr && _applicator->initializationRequired(configuration);
}