#include "stdafx.h"
#include "darksouls3profile.h"

#define DARKSOULS3_INITIALIZATION_DELAY 24

direct3d11::DarkSouls3Profile::DarkSouls3Profile(direct3d11::ProfileConfiguration configuration)
    : _configuration(configuration) {
    log(debug, ENCRYPT_STRING("created darksouls3 profile"));
}

void direct3d11::DarkSouls3Profile::outputMergerSetDepthStencilState(ID3D11DepthStencilState *depthStencilState, UINT stencil) {
}

void direct3d11::DarkSouls3Profile::outputMergerSetRenderTargets(UINT viewCount, ID3D11RenderTargetView * const *renderTargetViews, ID3D11DepthStencilView *depthStencilView) {
	if (depthStencilView != nullptr) {
		_depthStencilView = depthStencilView;
	}
}

void direct3d11::DarkSouls3Profile::pixelShaderSetShaderResources(UINT startSlot, UINT viewCount, ID3D11ShaderResourceView *const *shaderResourceViews) {
	while (viewCount > 0 && !_hasFoundFrame) {
		ID3D11Resource *resource = nullptr;
		ID3D11ShaderResourceView *shaderResourceView = shaderResourceViews[0];
		if (shaderResourceView != nullptr) {
			shaderResourceView->GetResource(&resource);
			auto resourceComponent = core::disposing::makeUnknown<ID3D11Resource>(resource);
			if (resource != nullptr) {
				auto texture = reinterpret_cast<ID3D11Texture2D*>(resource);
				if (texture != nullptr) {
                    auto depthTexture = direct3d11::utility::getDepthTexture(_depthStencilView);
                    direct3d11::dto::PostProcessingConfiguration configuration;
                    configuration.colorTexture = texture;
                    configuration.depthTexture = depthTexture.get();
                    auto initializationRequired = _configuration.applicator->initializationRequired(configuration);
                    if (!initializationRequired || _initializationDelayCount >= DARKSOULS3_INITIALIZATION_DELAY) {
                        _initializationDelayCount = 0;
                        _configuration.applicator->applyPostProcessing(configuration);
                    }
                    else if (_initializationDelayCount < DARKSOULS3_INITIALIZATION_DELAY) {
                        _initializationDelayCount++;
                    }

					_configuration.applicator->applyPostProcessing(configuration);
				}
			}
		}

        _hasFoundFrame = true;
	}
}

void direct3d11::DarkSouls3Profile::presentFrame() {
    _hasFoundFrame = false;
}