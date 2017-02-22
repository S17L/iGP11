#include "stdafx.h"
#include "igp11core.h"

enum FileType {
	DDS = 0,
	WIC = 1
};

enum Srgb {
	none = 0,
	in = 1,
	out = 2,
	both = 3
};

struct TextureMetadata {
	size_t height;
	size_t width;
	size_t mipmapsCount;
	unsigned int dxgiFormat;
};

struct TextureConversionSettings {
	size_t maxHeight;
	size_t maxWidth;
	unsigned int dxgiFormat;
	unsigned int colorSpace;
	bool keepMipMaps;
	unsigned int fileType;
};

struct TextureBuffer {
	void *buffer;
	size_t size;
	TextureMetadata metadata;
};

class ITextureService {
public:
	virtual ~ITextureService() {}
	virtual HRESULT convert(const char *sourceTexturePath, TextureConversionSettings *settings, TextureBuffer *texture) = 0;
	virtual HRESULT getTextureMetadata(const char *texturePath, TextureMetadata *textureMetadata) = 0;
	virtual void release(void *textureBuffer) = 0;
};

class TextureService : public ITextureService {
public:
	virtual ~TextureService() {}
	virtual HRESULT convert(const char *sourceTexturePath, TextureConversionSettings *settings, TextureBuffer *texture) override;
	virtual HRESULT getTextureMetadata(const char *texturePath, TextureMetadata *textureMetadata) override;
	virtual void release(void *textureBuffer) override;
};