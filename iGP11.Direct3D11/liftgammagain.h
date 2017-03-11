#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"

namespace direct3d11 {
    class LiftGammaGainEffect : public BaseEffect {
    private:
        ITexture *_colorTexture;
        core::dto::LiftGammaGain _liftGammaGain;
    public:
        LiftGammaGainEffect(Direct3D11Context *context, ITexture *colorTexture, dto::RenderingResolution resolution, core::dto::LiftGammaGain liftGammaGain, ShaderCodeFactory *codeFactory);
        virtual ~LiftGammaGainEffect() {}
    protected:
        virtual std::string getName() override;
        virtual ShaderCode getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}