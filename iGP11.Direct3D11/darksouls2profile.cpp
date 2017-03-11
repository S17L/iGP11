#include "stdafx.h"
#include "darksouls2profile.h"

#define DARKSOULS2_INITIALIZATION_DELAY 24
#define DARKSOULS2_PROFILE_COLOR_TEXTURE_INDEX 2
#define DARKSOULS2_PROFILE_COLOR_TEXTURE_FORMAT DXGI_FORMAT_R16G16B16A16_FLOAT

direct3d11::DarkSouls2Profile::DarkSouls2Profile(direct3d11::ProfileConfiguration configuration)
    : _configuration(configuration) {
    log(debug, ENCRYPT_STRING("created darksouls2 profile"));
}

void direct3d11::DarkSouls2Profile::outputMergerSetDepthStencilState(ID3D11DepthStencilState *depthStencilState, UINT stencil) {
}

void direct3d11::DarkSouls2Profile::outputMergerSetRenderTargets(UINT viewCount, ID3D11RenderTargetView * const *renderTargetViews, ID3D11DepthStencilView *depthStencilView) {
    if (depthStencilView != nullptr) {
        auto texture = direct3d11::utility::getDepthTexture(depthStencilView);
        if (texture != nullptr && direct3d11::utility::getDescription(texture.get()).Width == _resolution.width) {
            _depthStencilView = depthStencilView;
        }
    }
}

void direct3d11::DarkSouls2Profile::pixelShaderSetShaderResources(UINT startSlot, UINT viewCount, ID3D11ShaderResourceView * const *shaderResourceViews) {
    for (UINT i = 0; i < viewCount; i++) {
        ID3D11Resource *resource = nullptr;
        auto shaderResourceView = shaderResourceViews[i];
        if (shaderResourceView != nullptr) {
            shaderResourceView->GetResource(&resource);
            auto resourceWrapper = core::disposing::makeUnknown<ID3D11Resource>(resource);
            if (resource != nullptr) {
                auto texture = reinterpret_cast<ID3D11Texture2D*>(resource);
                if (texture != nullptr) {
                    auto description = direct3d11::utility::getDescription(texture);
                    if (description.Width == _resolution.width && description.MipLevels == 1 && description.ArraySize == 1 && description.Format == DARKSOULS2_PROFILE_COLOR_TEXTURE_FORMAT) {
                        _frameIndex++;
                        if (_frameIndex == DARKSOULS2_PROFILE_COLOR_TEXTURE_INDEX) {
                            auto depthTexture = direct3d11::utility::getDepthTexture(_depthStencilView);
                            direct3d11::dto::PostProcessingSettings postProcessingSettings;
                            postProcessingSettings.colorTexture = texture;
                            postProcessingSettings.depthTexture = depthTexture.get();
                            auto initializationRequired = _configuration.applicator->initializationRequired(postProcessingSettings);
                            if (!initializationRequired || _initializationDelayCount >= DARKSOULS2_INITIALIZATION_DELAY) {
                                _initializationDelayCount = 0;
                                _configuration.applicator->applyPostProcessing(postProcessingSettings);
                            }
                            else if (_initializationDelayCount < DARKSOULS2_INITIALIZATION_DELAY) {
                                _initializationDelayCount++;
                            }
                        }
                    }
                }
            }
        }
    }
}

void direct3d11::DarkSouls2Profile::presentFrame() {
    _frameIndex = 0;
    _resolution = direct3d11::utility::getRenderingResolution(_configuration.context->getChain());
}