#include "stdafx.h"
#include "horizontalgaussianblur.h"

direct3d11::HorizontalGaussianBlurEffect::HorizontalGaussianBlurEffect(Direct3D11Context *context, ID3D11ShaderResourceView *colorTextureView, dto::RenderingResolution resolution, dto::GaussianBlurConfiguration configuration, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTextureView = colorTextureView;
    _configuration = configuration;
}

std::string direct3d11::HorizontalGaussianBlurEffect::getName() {
    return ENCRYPT_STRING("direct3d11::HorizontalGaussianBlurEffect");
}

direct3d11::PassSettings direct3d11::HorizontalGaussianBlurEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createHorizontalGaussianBlurCode(_colorTextureView, _configuration.size, _configuration.sigma);
}