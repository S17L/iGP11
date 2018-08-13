#include "stdafx.h"
#include "verticalgaussianblur.h"

direct3d11::VerticalGaussianBlurEffect::VerticalGaussianBlurEffect(Direct3D11Context *context, ID3D11ShaderResourceView *colorTextureView, dto::RenderingResolution resolution, dto::GaussianBlurConfiguration configuration, ShaderCodeFactory *codeFactory)
    : direct3d11::BaseEffect(context, resolution, codeFactory) {
    _colorTextureView = colorTextureView;
    _configuration = configuration;
}

std::string direct3d11::VerticalGaussianBlurEffect::getName() {
    return ENCRYPT_STRING("direct3d11::VerticalGaussianBlurEffect");
}

direct3d11::PassSettings direct3d11::VerticalGaussianBlurEffect::getCode(direct3d11::Direct3D11Context *context, direct3d11::ShaderCodeFactory *codeFactory) {
    return codeFactory->createVerticalGaussianBlurCode(_colorTextureView, _configuration.size, _configuration.sigma);
}