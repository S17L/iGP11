#include "stdafx.h"
#include "denoise.h"

direct3d11::DenoiseEffect::DenoiseEffect(Direct3D11Context *context, ITexture *colorTexture, direct3d11::dto::RenderingResolution resolution, core::dto::Denoise denoise, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
    _denoise = denoise;
}

std::string direct3d11::DenoiseEffect::getName() {
    return ENCRYPT_STRING("direct3d11::DenoiseEffect");
}

direct3d11::ShaderCode direct3d11::DenoiseEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createDenoiseCode(_colorTexture->getShaderView(), _denoise);
}