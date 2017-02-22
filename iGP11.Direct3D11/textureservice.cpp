#include "stdafx.h"
#include "textureservice.h"
#include "igp11core.h"

#include "DDSTextureLoader.h"
#include "ScreenGrab.h"

HRESULT direct3d11::TextureService::saveTextureToFile(direct3d11::Direct3D11Context *context, ID3D11Resource *texture, std::string filePath) {
    auto path = core::toWString(filePath);
	auto deviceContext = context->getDeviceContext();

	return DirectX::SaveDDSTextureToFile(
        deviceContext.get(),
		texture,
        path.c_str());
}

HRESULT direct3d11::TextureService::createTextureFromFile(direct3d11::Direct3D11Context *context, ID3D11Resource **texture, ID3D11ShaderResourceView **textureView, std::string filePath, bool forceSrgb) {
    auto path = core::toWString(filePath);
    auto device = context->getDevice();
    
    return DirectX::CreateDDSTextureFromFileEx(
		device.get(),
        path.c_str(),
		0,
		D3D11_USAGE_DEFAULT,
		D3D11_BIND_SHADER_RESOURCE,
		0,
		0,
        forceSrgb,
		texture,
		textureView,
		nullptr);
}