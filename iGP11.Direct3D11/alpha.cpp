#include "stdafx.h"
#include "alpha.h"

direct3d11::AlphaEffect::AlphaEffect(Direct3D11Context *context, ITexture *colorTexture, direct3d11::dto::RenderingResolution resolution, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
}

std::string direct3d11::AlphaEffect::getName() {
    return ENCRYPT_STRING("direct3d11::AlphaEffect");
}

direct3d11::PassSettings direct3d11::AlphaEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createAlphaCode(_colorTexture->getShaderView());
}