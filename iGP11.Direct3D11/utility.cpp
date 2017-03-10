#include "stdafx.h"
#include "utility.h"

std::map<DXGI_FORMAT, DXGI_FORMAT> _depthFormatTranslator = {
    { DXGI_FORMAT_R16_TYPELESS, DXGI_FORMAT_R16_FLOAT },
    { DXGI_FORMAT_R32_TYPELESS, DXGI_FORMAT_R32_FLOAT },
    { DXGI_FORMAT_R24G8_TYPELESS, DXGI_FORMAT_R24_UNORM_X8_TYPELESS },
    { DXGI_FORMAT_R32G8X24_TYPELESS, DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS }
};

std::string direct3d11::stringify::toString(const D3D11_TEXTURE2D_DESC *description) {
    if (description != nullptr) {
        return core::stringFormat(
            "[ width: %u, height: %u, miplevels: %u, arraysize: %u, format: %u, sample: %s, usage: %u, bindflags: %u, cpuaccessflags: %u, miscflags: %u ]",
            description->Width,
            description->Height,
            description->MipLevels,
            description->ArraySize,
            description->Format,
            direct3d11::stringify::toString(&description->SampleDesc).c_str(),
            description->Usage,
            description->BindFlags,
            description->CPUAccessFlags,
            description->MiscFlags);
    }
    else {
        return ENCRYPT_STRING("[]");
    }
}

std::string direct3d11::stringify::toString(const D3D11_SUBRESOURCE_DATA *data) {
    if (data != nullptr) {
        return core::stringFormat(
            ENCRYPT_STRING("[ sysmempitch: %d, sysmemslicepitch: %d ]"),
            data->SysMemPitch,
            data->SysMemSlicePitch);
    }
    else {
        return ENCRYPT_STRING("[]");
    }
}

std::string direct3d11::stringify::toString(const DXGI_MODE_DESC *description) {
    if (description != nullptr) {
        return core::stringFormat(
            "[ width: %u, height: %u, refreshRate: %s, format: %d, scanlineOrdering: %d, scaling: %d ]",
            description->Width,
            description->Height,
            direct3d11::stringify::toString(&description->RefreshRate).c_str(),
            description->Format,
            description->ScanlineOrdering,
            description->Scaling);
    }
    else {
        return ENCRYPT_STRING("[]");
    }
}

std::string direct3d11::stringify::toString(const DXGI_RATIONAL *rational) {
    if (rational != nullptr) {
        return core::stringFormat(
            ENCRYPT_STRING("[ numerator: %u, denominator: %u ]"),
            rational->Numerator,
            rational->Denominator);
    }
    else {
        return ENCRYPT_STRING("[]");
    }
}

std::string direct3d11::stringify::toString(const DXGI_SAMPLE_DESC *description) {
    if (description != nullptr) {
        return core::stringFormat(
            ENCRYPT_STRING("[ count: %u, quality: %u ]"),
            description->Count,
            description->Quality);
    }
    else {
        return ENCRYPT_STRING("[]");
    }
}

std::string direct3d11::stringify::toString(const DXGI_SWAP_CHAIN_DESC *description) {
    if (description != nullptr) {
        return core::stringFormat(
            "[ bufferDescription: %s, sampleDescription: %s, bufferUsage: %u, bufferCount: %u, outputWindow: %p, windowed: %d, swapEffect: %d, flags: %u ]",
            direct3d11::stringify::toString(&description->BufferDesc).c_str(),
            direct3d11::stringify::toString(&description->SampleDesc).c_str(),
            description->BufferUsage,
            description->BufferCount,
            description->OutputWindow,
            description->Windowed ? 1 : 0,
            description->SwapEffect,
            description->Flags);
    }
    else {
        return ENCRYPT_STRING("[]");
    }
}

D3D11_TEXTURE2D_DESC direct3d11::utility::createFloatTextureDescription(D3D11_TEXTURE2D_DESC textureDescription) {
    textureDescription.Format = DXGI_FORMAT_R16G16B16A16_FLOAT;
    return textureDescription;
}

DXGI_FORMAT direct3d11::utility::getDepthTextureViewFormat(DXGI_FORMAT textureFormat) {
    auto iterator = _depthFormatTranslator.find(textureFormat);
    if (iterator == _depthFormatTranslator.end()) {
        throw core::exception::OperationException(ENCRYPT_STRING("direct3d11::utility::getDepthTextureViewFormat"), core::stringFormat(ENCRYPT_STRING("format: %d could not be mapped"), static_cast<int>(textureFormat)));
    }

    return iterator->second;
}

DXGI_SWAP_CHAIN_DESC direct3d11::utility::getDescription(IDXGISwapChain *chain) {
    DXGI_SWAP_CHAIN_DESC description;
    ZeroMemory(&description, sizeof(DXGI_SWAP_CHAIN_DESC));
    HRESULT result = chain->GetDesc(&description);
    if (FAILED(result)) {
        throw core::exception::OperationException(ENCRYPT_STRING("direct3d11::utility::getDescription"), ENCRYPT_STRING("swap chain description could not be obtained"));
    }

    return description;
}

D3D11_TEXTURE2D_DESC direct3d11::utility::getDescription(ID3D11Texture2D *texture) {
    D3D11_TEXTURE2D_DESC description;
    ZeroMemory(&description, sizeof(D3D11_TEXTURE2D_DESC));
    texture->GetDesc(&description);

    return description;
}

D3D11_DEPTH_STENCIL_DESC direct3d11::utility::getDescription(ID3D11DepthStencilState *depthStencilState) {
    D3D11_DEPTH_STENCIL_DESC description;
    ZeroMemory(&description, sizeof(D3D11_DEPTH_STENCIL_DESC));
    depthStencilState->GetDesc(&description);

    return description;
}

direct3d11::dto::GaussianBlurConfiguration direct3d11::utility::getGaussianBlurConfiguration(float smoothness) {
    const float coefficient = 5;
    const unsigned int size = 3;

    return direct3d11::dto::GaussianBlurConfiguration(size, smoothness * coefficient);
}

direct3d11::dto::RenderingResolution direct3d11::utility::getRenderingResolution(ID3D11Texture2D *texture) {
    auto description = direct3d11::utility::getDescription(texture);
    direct3d11::dto::RenderingResolution resolution;
    resolution.height = description.Height;
    resolution.width = description.Width;

    return resolution;
}

direct3d11::dto::RenderingResolution direct3d11::utility::getRenderingResolution(IDXGISwapChain *chain) {
    auto description = direct3d11::utility::getDescription(chain);
    direct3d11::dto::RenderingResolution resolution;
    resolution.height = description.BufferDesc.Height;
    resolution.width = description.BufferDesc.Width;

    return resolution;
}

core::disposing::unique_ptr<ID3D11Texture2D> direct3d11::utility::getBackBuffer(IDXGISwapChain *chain) {
    ID3D11Texture2D *texture = nullptr;
    auto result = chain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&texture);
    if (FAILED(result)) {
        throw core::exception::OperationException(ENCRYPT_STRING("direct3d11::utility::getBackBuffer"), ENCRYPT_STRING("back buffer could not be obtained"));
    }

    return core::disposing::makeUnknown<ID3D11Texture2D>(texture);
}

core::disposing::unique_ptr<ID3D11Texture2D> direct3d11::utility::getDepthTexture(ID3D11DepthStencilView *depthStencilView) {
    if (depthStencilView != nullptr) {
        ID3D11Resource *resource = nullptr;
        depthStencilView->GetResource(&resource);
        if (resource != nullptr) {
            auto texture = reinterpret_cast<ID3D11Texture2D*>(resource);
            if (texture != nullptr) {
                return core::disposing::makeUnknown<ID3D11Texture2D>(texture);
            }
        }

        SAFE_RELEASE(resource);
    }

    return core::disposing::makeDummy<ID3D11Texture2D>(nullptr);
}

void direct3d11::utility::setNoRenderer(direct3d11::Direct3D11Context *context) {
    auto deviceContext = context->getDeviceContext();
    deviceContext->OMSetRenderTargets(0, nullptr, nullptr);
}