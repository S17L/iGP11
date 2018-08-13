#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"

namespace direct3d11 {
    namespace stringify {
        std::string toString(const ITexture *texture);
        std::string toString(const D3D11_TEXTURE2D_DESC *description);
        std::string toString(const D3D11_SUBRESOURCE_DATA *data);
        std::string toString(const DXGI_MODE_DESC *description);
        std::string toString(const DXGI_RATIONAL *rational);
        std::string toString(const DXGI_SAMPLE_DESC *description);
        std::string toString(const DXGI_SWAP_CHAIN_DESC *description);
    }

	namespace utility {
        D3D11_TEXTURE2D_DESC apply(D3D11_TEXTURE2D_DESC description, core::dto::Resolution resolution);
        D3D11_TEXTURE2D_DESC createFloatTextureDescription(D3D11_TEXTURE2D_DESC textureDescription);
        DXGI_FORMAT getDepthTextureViewFormat(DXGI_FORMAT textureFormat);
		DXGI_SWAP_CHAIN_DESC getDescription(IDXGISwapChain *chain);
		D3D11_TEXTURE2D_DESC getDescription(ID3D11Texture2D *texture);
        D3D11_DEPTH_STENCIL_DESC getDescription(ID3D11DepthStencilState *depthStencilState);
        core::dto::Resolution getRenderingResolution(ID3D11Texture2D *texture);
        core::dto::Resolution getRenderingResolution(IDXGISwapChain *chain);
		core::disposing::unique_ptr<ID3D11Texture2D> getBackBuffer(IDXGISwapChain *chain);
        core::disposing::unique_ptr<ID3D11Texture2D> getDepthTexture(ID3D11DepthStencilView *depthStencilView);
		void setNoRenderer(direct3d11::Direct3D11Context *context);
	}
}