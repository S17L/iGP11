#include "stdafx.h"
#include "genericprofile.h"

direct3d11::GenericProfile::GenericProfile(direct3d11::ProfileConfiguration configuration)
    : _configuration(configuration) {
    log(debug, ENCRYPT_STRING("created generic profile"));
}

void direct3d11::GenericProfile::outputMergerSetDepthStencilState(ID3D11DepthStencilState *depthStencilState, UINT stencil) {
}

void direct3d11::GenericProfile::outputMergerSetRenderTargets(UINT viewCount, ID3D11RenderTargetView * const *renderTargetViews, ID3D11DepthStencilView *depthStencilView) {
    if (depthStencilView != nullptr) {
        _depthStencilView = depthStencilView;
    }
}

void direct3d11::GenericProfile::pixelShaderSetShaderResources(UINT startSlot, UINT viewCount, ID3D11ShaderResourceView * const *shaderResourceViews) {
}

void direct3d11::GenericProfile::presentFrame() {
    auto colorTexture = direct3d11::utility::getBackBuffer(_configuration.context->getChain());
    auto depthTexture = direct3d11::utility::getDepthTexture(_depthStencilView);

    direct3d11::dto::PostProcessingSettings postProcessingSettings;
    postProcessingSettings.colorTexture = colorTexture.get();
    postProcessingSettings.depthTexture = depthTexture.get();
    _configuration.applicator->applyPostProcessing(postProcessingSettings);
}