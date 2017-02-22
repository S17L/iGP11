#include "stdafx.h"
#include "igp11direct3d11.h"

namespace direct3d11 {
	class TextureService : public direct3d11::ITextureService {
	public:
		virtual ~TextureService() {}
		virtual HRESULT saveTextureToFile(direct3d11::Direct3D11Context *context, ID3D11Resource *texture, std::string filePath) override;
		virtual HRESULT createTextureFromFile(direct3d11::Direct3D11Context *context, ID3D11Resource **texture, ID3D11ShaderResourceView **textureView, std::string filePath, bool forceSrgb) override;
	};
}