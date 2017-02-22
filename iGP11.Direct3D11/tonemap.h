#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"

namespace direct3d11 {
    class TonemapEffect : public BaseEffect {
    private:
        ITexture *_colorTexture;
        core::dto::Tonemap _tonemap;
    public:
        TonemapEffect(Direct3D11Context *context, ITexture *colorTexture, dto::RenderingResolution resolution, core::dto::Tonemap tonemap, ShaderCodeFactory *codeFactory);
        virtual ~TonemapEffect() {}
    protected:
        virtual std::string getName() override;
        virtual ShaderCode getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}