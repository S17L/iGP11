#include "stdafx.h"
#include "tonemap.h"

direct3d11::TonemapEffect::TonemapEffect(Direct3D11Context *context, ITexture *colorTexture, direct3d11::dto::RenderingResolution resolution, core::dto::Tonemap tonemap, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
    _tonemap = tonemap;
}

std::string direct3d11::TonemapEffect::getName() {
    return ENCRYPT_STRING("direct3d11::TonemapEffect");
}

direct3d11::PassSettings direct3d11::TonemapEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createTonemapCode(_colorTexture->getShaderView(), _tonemap);
}