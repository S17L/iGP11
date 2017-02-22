#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"

namespace direct3d11 {
    class LumasharpenEffect : public BaseEffect {
    private:
        ITexture *_colorTexture;
        core::dto::LumaSharpen _lumaSharpen;
    public:
        LumasharpenEffect(Direct3D11Context *context, ITexture *colorTexture, dto::RenderingResolution resolution, core::dto::LumaSharpen lumaSharpen, ShaderCodeFactory *codeFactory);
        virtual ~LumasharpenEffect() {}
    protected:
        virtual std::string getName() override;
        virtual ShaderCode getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}