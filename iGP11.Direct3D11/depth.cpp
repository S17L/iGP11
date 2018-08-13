#include "stdafx.h"
#include "depth.h"

direct3d11::DepthEffect::DepthEffect(direct3d11::Direct3D11Context *context, ITexture *colorTexture, ID3D11ShaderResourceView *depthTextureView, direct3d11::dto::RenderingResolution resolution, core::dto::DepthBuffer depthBuffer, direct3d11::ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
    _depthTextureView = depthTextureView;
    _depthBuffer = depthBuffer;
}

std::string direct3d11::DepthEffect::getName() {
    return ENCRYPT_STRING("direct3d11::DepthEffect");
}

direct3d11::PassSettings direct3d11::DepthEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createDepthRenderingCode(_colorTexture->getShaderView(), _depthTextureView, _depthBuffer);
}