#include "stdafx.h"
#include "luminescence.h"

direct3d11::LuminescenceEffect::LuminescenceEffect(Direct3D11Context *context, ITexture *colorTexture, direct3d11::dto::RenderingResolution resolution, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
}

std::string direct3d11::LuminescenceEffect::getName() {
    return ENCRYPT_STRING("direct3d11::LuminescenceEffect");
}

direct3d11::PassSettings direct3d11::LuminescenceEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createLuminescenceCode(_colorTexture->getShaderView());
}