#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "utility.h"

using namespace core::logging;

namespace direct3d11 {
	class DarkSouls3Profile : public IProfile {
	private:
        direct3d11::ProfileConfiguration _configuration;
        core::dto::Resolution _resolution;
		bool _hasFoundFrame = false;
        unsigned int _initializationDelayCount = 0;
		ID3D11DepthStencilView *_depthStencilView = nullptr;
	public:
        DarkSouls3Profile(direct3d11::ProfileConfiguration configuration);
		virtual ~DarkSouls3Profile() {}
        virtual void outputMergerSetDepthStencilState(ID3D11DepthStencilState *depthStencilState, UINT stencil) override;
        virtual void outputMergerSetRenderTargets(UINT viewCount, ID3D11RenderTargetView * const *renderTargetViews, ID3D11DepthStencilView *depthStencilView) override;
        virtual void pixelShaderSetShaderResources(UINT startSlot, UINT viewCount, ID3D11ShaderResourceView *const *shaderResourceViews) override;
        virtual void presentFrame() override;
	};
}