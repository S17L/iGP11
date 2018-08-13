#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"

namespace direct3d11 {
    class VibranceEffect : public BaseEffect {
    private:
        ITexture *_colorTexture;
        core::dto::Vibrance _vibrance;
    public:
        VibranceEffect(Direct3D11Context *context, ITexture *colorTexture, dto::RenderingResolution resolution, core::dto::Vibrance vibrance, ShaderCodeFactory *codeFactory);
        virtual ~VibranceEffect() {}
    protected:
        virtual std::string getName() override;
        virtual PassSettings getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}