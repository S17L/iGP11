#include "stdafx.h"
#include "textureservice.h"

using namespace std;

#define DLL __declspec(dllexport)

unique_ptr<ITextureService> _textureService;

#ifdef __cplusplus
extern "C"
{
#endif

	DLL HRESULT ConvertTexture(
		const char *sourceTexturePath,
		TextureConversionSettings *settings,
		TextureBuffer *texture) {
		return _textureService->convert(
			sourceTexturePath,
			settings,
			texture);
	}

	DLL HRESULT GetTextureMetadata(
		const char *texturePath,
		TextureMetadata *textureMetadata) {
		return _textureService->getTextureMetadata(
			texturePath,
			textureMetadata);
	}

	DLL void ReleaseTextureBuffer(void *textureBuffer) {
		_textureService->release(textureBuffer);
	}

#ifdef __cplusplus
}
#endif

BOOL APIENTRY DllMain(HINSTANCE module, DWORD reason, LPVOID reserved) {
	if (reason == DLL_PROCESS_ATTACH) {
		_textureService.reset(new TextureService());
	} else if (reason == DLL_PROCESS_DETACH) {
		_textureService.reset();
	}

	return TRUE;
}

