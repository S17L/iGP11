#include "stdafx.h"
#include "lumasharpen.h"

direct3d11::LumasharpenEffect::LumasharpenEffect(Direct3D11Context *context, ITexture *colorTexture, direct3d11::dto::RenderingResolution resolution, core::dto::LumaSharpen lumaSharpen, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
    _lumaSharpen = lumaSharpen;
}

std::string direct3d11::LumasharpenEffect::getName() {
    return ENCRYPT_STRING("direct3d11::LumasharpenEffect");
}

direct3d11::PassSettings direct3d11::LumasharpenEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createLumaSharpenCode(_colorTexture->getShaderView(), _lumaSharpen);
}