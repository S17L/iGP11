#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"

namespace direct3d11 {
    class DenoiseEffect : public BaseEffect {
    private:
        ITexture *_colorTexture;
        core::dto::Denoise _denoise;
    public:
        DenoiseEffect(Direct3D11Context *context, ITexture *colorTexture, dto::RenderingResolution resolution, core::dto::Denoise denoise, ShaderCodeFactory *codeFactory);
        virtual ~DenoiseEffect() {}
    protected:
        virtual std::string getName() override;
        virtual PassSettings getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}