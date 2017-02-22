#include "stdafx.h"
#include "vibrance.h"

direct3d11::VibranceEffect::VibranceEffect(Direct3D11Context *context, ITexture *colorTexture, direct3d11::dto::RenderingResolution resolution, core::dto::Vibrance vibrance, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
    _vibrance = vibrance;
}

std::string direct3d11::VibranceEffect::getName() {
    return ENCRYPT_STRING("direct3d11::VibranceEffect");
}

direct3d11::ShaderCode direct3d11::VibranceEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createVibranceCode(_colorTexture->getShaderView(), _vibrance);
}