#include "stdafx.h"
#include "igp11direct3d11.h"

IDXGISwapChain* direct3d11::Direct3D11Context::getChain() {
    return _chain;
}

core::disposing::unique_ptr<ID3D11Device> direct3d11::Direct3D11Context::getDevice() {
    ID3D11Device *device = nullptr;
    HRESULT result = _chain->GetDevice(__uuidof(ID3D11Device), (LPVOID*)&device);
    if (FAILED(result) || device == nullptr) {
        throw core::exception::OperationException(ENCRYPT_STRING("direct3d11::direct3d11context"), ENCRYPT_STRING("device could not be obtained"));
    }

    return core::disposing::makeUnknown<ID3D11Device>(device);
}

core::disposing::unique_ptr<ID3D11DeviceContext> direct3d11::Direct3D11Context::getDeviceContext() {
    ID3D11DeviceContext *deviceContext = nullptr;
    core::disposing::unique_ptr<ID3D11Device> device = getDevice();
    device->GetImmediateContext(&deviceContext);
    if (deviceContext == nullptr) {
        throw core::exception::OperationException(ENCRYPT_STRING("direct3d11::direct3d11context"), ENCRYPT_STRING("device context could not be obtained"));
    }

    return core::disposing::makeUnknown<ID3D11DeviceContext>(deviceContext);
}