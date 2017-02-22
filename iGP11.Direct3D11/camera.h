#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace direct3d11 {
    namespace camera {
        void getCamera(DirectX::XMMATRIX &matrix, float px, float py, float pz, float rx = 0, float ry = 0, float rz = 0);
        void getOrtho(DirectX::XMMATRIX &matrix, float width, float height, float near, float depth);
        void getWorld(DirectX::XMMATRIX &matrix);
    }
}