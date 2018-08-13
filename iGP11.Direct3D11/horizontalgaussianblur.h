#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"
#include "utility.h"

namespace direct3d11 {
    class HorizontalGaussianBlurEffect : public BaseEffect {
    private:
        ID3D11ShaderResourceView *_colorTextureView;
        dto::GaussianBlurConfiguration _configuration;
    public:
        HorizontalGaussianBlurEffect(Direct3D11Context *context, ID3D11ShaderResourceView *colorTextureView, dto::RenderingResolution resolution, dto::GaussianBlurConfiguration configuration, ShaderCodeFactory *codeFactory);
        virtual ~HorizontalGaussianBlurEffect() {}
    protected:
        virtual std::string getName() override;
        virtual PassSettings getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}