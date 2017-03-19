#include "stdafx.h"
#include "hdr.h"

direct3d11::HDREffect::HDREffect(Direct3D11Context *context, ITexture *colorTexture, direct3d11::dto::RenderingResolution resolution, core::dto::HDR hdr, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
    _hdr = hdr;
}

std::string direct3d11::HDREffect::getName() {
    return ENCRYPT_STRING("direct3d11::HDREffect");
}

direct3d11::ShaderCode direct3d11::HDREffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createHDRCode(_colorTexture->getShaderView(), _hdr);
}