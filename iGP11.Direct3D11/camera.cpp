#include "stdafx.h"
#include "camera.h"

void direct3d11::camera::getCamera(DirectX::XMMATRIX &matrix, float px, float py, float pz, float rx, float ry, float rz) {
    DirectX::XMFLOAT3 up;
    up.x = 0.0f;
    up.y = 1.0f;
    up.z = 0.0f;

    DirectX::XMFLOAT3 position;
    position.x = px;
    position.y = py;
    position.z = pz;

    DirectX::XMFLOAT3 lookAt;
    lookAt.x = 0.0f;
    lookAt.y = 0.0f;
    lookAt.z = 1.0f;

    float pitch = rx * 0.0174532925f;
    float yaw = ry * 0.0174532925f;
    float roll = rz * 0.0174532925f;

    DirectX::XMMATRIX rotationMatrix = DirectX::XMMatrixRotationRollPitchYaw(pitch, yaw, roll);
    DirectX::XMVECTOR lookAtVector = DirectX::XMVector3TransformCoord(DirectX::XMLoadFloat3(&lookAt), rotationMatrix);
    DirectX::XMVECTOR upVector = DirectX::XMVector3TransformCoord(DirectX::XMLoadFloat3(&up), rotationMatrix);

    lookAtVector = DirectX::XMVectorAdd(DirectX::XMLoadFloat3(&position), lookAtVector);
    matrix = DirectX::XMMatrixLookAtLH(DirectX::XMLoadFloat3(&position), lookAtVector, upVector);
}

void direct3d11::camera::getOrtho(DirectX::XMMATRIX &matrix, float width, float height, float viewNear, float viewDepth) {
    matrix = DirectX::XMMatrixOrthographicLH(width, height, viewNear, viewDepth);
}

void direct3d11::camera::getWorld(DirectX::XMMATRIX &matrix) {
    matrix = DirectX::XMMatrixIdentity();
}