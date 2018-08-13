#include "stdafx.h"
#include "liftgammagain.h"

direct3d11::LiftGammaGainEffect::LiftGammaGainEffect(Direct3D11Context *context, ITexture *colorTexture, direct3d11::dto::RenderingResolution resolution, core::dto::LiftGammaGain liftGammaGain, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTexture = colorTexture;
    _liftGammaGain = liftGammaGain;
}

std::string direct3d11::LiftGammaGainEffect::getName() {
    return ENCRYPT_STRING("direct3d11::LiftGammaGainEffect");
}

direct3d11::PassSettings direct3d11::LiftGammaGainEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createLiftGammaGainCode(_colorTexture->getShaderView(), _liftGammaGain);
}