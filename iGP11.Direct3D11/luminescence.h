#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"

namespace direct3d11 {
    class LuminescenceEffect : public BaseEffect {
    private:
        ITexture *_colorTexture;
    public:
        LuminescenceEffect(Direct3D11Context *context, ITexture *colorTexture, dto::RenderingResolution resolution, ShaderCodeFactory *codeFactory);
        virtual ~LuminescenceEffect() {}
    protected:
        virtual std::string getName() override;
        virtual PassSettings getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}