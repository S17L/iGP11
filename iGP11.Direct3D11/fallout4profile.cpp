#include "stdafx.h"
#include "fallout4profile.h"

#define FALLOUT4_DUMPING_ALL_RESOURCES_BUFFER_INDEX 1
#define FALLOUT4_INITIALIZATION_DELAY 24
#define FALLOUT4_PROFILE_COLOR_TEXTURE_FORMAT DXGI_FORMAT_R11G11B10_FLOAT

direct3d11::Fallout4Profile::Fallout4Profile(direct3d11::ProfileConfiguration configuration)
    : _configuration(configuration) {
    log(debug, ENCRYPT_STRING("created fallout4 profile"));
}

void direct3d11::Fallout4Profile::outputMergerSetDepthStencilState(ID3D11DepthStencilState *depthStencilState, UINT stencil) {
}

void direct3d11::Fallout4Profile::outputMergerSetRenderTargets(UINT viewCount, ID3D11RenderTargetView * const *renderTargetViews, ID3D11DepthStencilView *depthStencilView) {
    if (depthStencilView != nullptr) {
        _depthStencilView = depthStencilView;
    }
}

void direct3d11::Fallout4Profile::pixelShaderSetShaderResources(UINT startSlot, UINT viewCount, ID3D11ShaderResourceView * const *shaderResourceViews) {
    if (_hasFoundFrame <= FALLOUT4_DUMPING_ALL_RESOURCES_BUFFER_INDEX && startSlot == 0 && viewCount == 1) {
        auto shaderResourceView = shaderResourceViews[0];
        if (shaderResourceView != nullptr) {
            ID3D11Resource *resource = nullptr;
            shaderResourceView->GetResource(&resource);
            auto resourceComponent = core::disposing::makeUnknown<ID3D11Resource>(resource);
            if (resource != nullptr) {
                auto texture = reinterpret_cast<ID3D11Texture2D*>(resource);
                if (texture != nullptr) {
                    auto description = direct3d11::utility::getDescription(texture);
                    if (description.Width == _resolution.width && description.Height == _resolution.height && description.MipLevels == 1 && description.ArraySize == 1 && description.Format == FALLOUT4_PROFILE_COLOR_TEXTURE_FORMAT) {
                        _hasFoundFrame++;
                        if (_hasFoundFrame == FALLOUT4_DUMPING_ALL_RESOURCES_BUFFER_INDEX) {
                            auto depthTexture = direct3d11::utility::getDepthTexture(_depthStencilView);
                            direct3d11::dto::PostProcessingConfiguration configuration;
                            configuration.colorTexture = texture;
                            configuration.depthTexture = depthTexture.get();
                            auto initializationRequired = _configuration.applicator->initializationRequired(configuration);
                            if (!initializationRequired || _initializationDelayCount >= FALLOUT4_INITIALIZATION_DELAY) {
                                _initializationDelayCount = 0;
                                _configuration.applicator->applyPostProcessing(configuration);
                            }
                            else if (_initializationDelayCount < FALLOUT4_INITIALIZATION_DELAY) {
                                _initializationDelayCount++;
                            }
                        }
                    }
                }
            }
        }
    }
}

void direct3d11::Fallout4Profile::presentFrame() {
    _hasFoundFrame = 0;
    _resolution = direct3d11::utility::getRenderingResolution(_configuration.context->getChain());
}