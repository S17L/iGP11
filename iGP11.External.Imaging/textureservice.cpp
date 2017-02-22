#include "stdafx.h"
#include "textureservice.h"

#include "DirectXTex.h"

class ImageWrapper{
private:
	DirectX::Image *_images;
public:
	ImageWrapper(DirectX::Image *images)
		: _images(images) {}
	~ImageWrapper() {
		delete[] _images;
	}
	DirectX::Image* get() const {
		return _images;
	}
};

DirectX::TEX_FILTER_FLAGS translate(const Srgb &srgb) {
	switch (srgb) {
	case in:
		return DirectX::TEX_FILTER_SRGB_IN;
	case out:
		return DirectX::TEX_FILTER_SRGB_OUT;
	case both:
		return DirectX::TEX_FILTER_SRGB;
	default:
		return DirectX::TEX_FILTER_DEFAULT;
	}
}

void map(TextureMetadata &textureMetadata, const DirectX::TexMetadata &metadata) {
	textureMetadata.width = metadata.width;
	textureMetadata.height = metadata.height;
	textureMetadata.mipmapsCount = metadata.mipLevels;
	textureMetadata.dxgiFormat = metadata.format;
}

size_t roundDown(size_t number) {
	size_t power = 1;
	while (power * 2 <= number)
		power *= 2;

	return power;
}

void makeUNORM(DirectX::TexMetadata &metadata) {
	metadata.format = DirectX::MakeTypelessUNORM(metadata.format);
}

void makeTYPELESS(DirectX::TexMetadata &metadata) {
	metadata.format = DirectX::MakeTypeless(metadata.format);
}

DirectX::Image* update(const DirectX::Image *image, size_t count, std::function<void(DirectX::Image *image)> converter) {
	DirectX::Image *images = new DirectX::Image[count];
	if (image) {
		for (size_t i = 0; i < count; i++) {
			DirectX::Image *newImage = new DirectX::Image(image[i]);
			converter(newImage);
			images[i] = *newImage;
		}
	}

	return images;
}

DirectX::Image* makeUNORM(const DirectX::Image *image, size_t count) {
	return update(image, count, [](DirectX::Image *image) { image->format = DirectX::MakeTypelessUNORM(image->format); });
}

DirectX::Image* makeTYPELESS(const DirectX::Image *image, size_t count) {
	return update(image, count, [](DirectX::Image *image) { image->format = DirectX::MakeTypeless(image->format); });
}

HRESULT TextureService::convert(
	const char *sourceTexturePath,
	TextureConversionSettings *settings,
	TextureBuffer *texture) {
	DirectX::TexMetadata metadata;
	std::unique_ptr<DirectX::ScratchImage> outputImage(new DirectX::ScratchImage());
	std::unique_ptr<DirectX::ScratchImage> temporaryImage(new DirectX::ScratchImage());

	HRESULT result = DirectX::LoadFromDDSFile(
		core::toWString(std::string(sourceTexturePath)).c_str(),
		DirectX::DDS_FLAGS_FORCE_RGB | DirectX::DDS_FLAGS_NO_16BPP | DirectX::DDS_FLAGS_EXPAND_LUMINANCE,
		&metadata,
		*outputImage);

	const float threshold = 0.5f;
	const DXGI_FORMAT primaryDxgiFormat = DXGI_FORMAT_R8G8B8A8_UNORM;
	const DXGI_FORMAT secondaryDxgiFormat = DXGI_FORMAT_B8G8R8A8_UNORM;
	DXGI_FORMAT dxgiFormat = static_cast<DXGI_FORMAT>(settings->dxgiFormat);
    Srgb srgb = static_cast<Srgb>(settings->colorSpace);

	makeUNORM(metadata);
	DirectX::TEX_FILTER_FLAGS filter = translate(srgb);

	if (SUCCEEDED(result)) {
		if (DirectX::IsCompressed(metadata.format)) {
			if (settings->keepMipMaps) {
				result = Decompress(
					outputImage->GetImages(),
					outputImage->GetImageCount(),
					metadata,
					DXGI_FORMAT_UNKNOWN,
					*temporaryImage);
			} else {
				result = Decompress(
					*outputImage->GetImage(0, 0, 0),
					DXGI_FORMAT_UNKNOWN,
					*temporaryImage);
			}

			if (SUCCEEDED(result)) {
				metadata = temporaryImage->GetMetadata();
				outputImage = std::move(temporaryImage);
				temporaryImage.reset(new DirectX::ScratchImage());
			}
		}
	}

	if (SUCCEEDED(result) && DirectX::IsPlanar(metadata.format)) {
		if (settings->keepMipMaps) {
			result = DirectX::ConvertToSinglePlane(
				outputImage->GetImages(),
				outputImage->GetImageCount(),
				metadata,
				*temporaryImage);
		} else {
			result = DirectX::ConvertToSinglePlane(
				*outputImage->GetImage(0, 0, 0),
				*temporaryImage);
		}

		if (SUCCEEDED(result)) {
			metadata = temporaryImage->GetMetadata();
			outputImage = std::move(temporaryImage);
			temporaryImage.reset(new DirectX::ScratchImage());
		}
	}

	if (SUCCEEDED(result) && settings->maxHeight > 0 && settings->maxWidth > 0) {
		double ratioX = min((double)settings->maxWidth / metadata.width, 1);
		double ratioY = min((double)settings->maxHeight / metadata.height, 1);
		double ratio = min(ratioX, ratioY);
		size_t width = (size_t)(metadata.width * ratio);
		size_t height = (size_t)(metadata.height * ratio);
		width = ::roundDown(width);
		height = ::roundDown(height);

		if (settings->keepMipMaps) {
			ImageWrapper imageWrapper(makeUNORM(outputImage->GetImages(), outputImage->GetImageCount()));
			result = DirectX::Resize(
				imageWrapper.get(),
				outputImage->GetImageCount(),
				metadata,
				width,
				height,
				DirectX::TEX_FILTER_DEFAULT,
				*temporaryImage);
		} else {
			ImageWrapper imageWrapper(makeUNORM(outputImage->GetImage(0, 0, 0), 1));
			result = DirectX::Resize(
				*imageWrapper.get(),
				width,
				height,
				DirectX::TEX_FILTER_DEFAULT,
				*temporaryImage);
		}

		if (SUCCEEDED(result)) {
			metadata = temporaryImage->GetMetadata();
			outputImage = std::move(temporaryImage);
			temporaryImage.reset(new DirectX::ScratchImage());
		}
	}

	if (SUCCEEDED(result) && filter != DirectX::TEX_FILTER_DEFAULT) {
		DXGI_FORMAT destinationFormat = !DirectX::IsCompressed(dxgiFormat) && metadata.format != dxgiFormat
			? dxgiFormat
			: metadata.format != primaryDxgiFormat ? primaryDxgiFormat : secondaryDxgiFormat;

		if (settings->keepMipMaps) {
			ImageWrapper imageWrapper(makeUNORM(outputImage->GetImages(), outputImage->GetImageCount()));
			result = DirectX::Convert(
				imageWrapper.get(),
				outputImage->GetImageCount(),
				metadata,
				destinationFormat,
				filter,
				threshold,
				*temporaryImage);
		} else {
			ImageWrapper imageWrapper(makeUNORM(outputImage->GetImage(0, 0, 0), 1));
			result = DirectX::Convert(
				*imageWrapper.get(),
				destinationFormat,
				filter,
				threshold,
				*temporaryImage);
		}

		if (SUCCEEDED(result)) {
			metadata = temporaryImage->GetMetadata();
			outputImage = std::move(temporaryImage);
			temporaryImage.reset(new DirectX::ScratchImage());
		}
	}

	if (SUCCEEDED(result)) {
		if (DirectX::IsCompressed(dxgiFormat)) {
			ImageWrapper imageWrapper(makeUNORM(outputImage->GetImages(), outputImage->GetImageCount()));
			result = DirectX::Compress(
				imageWrapper.get(),
				outputImage->GetImageCount(),
				metadata,
				dxgiFormat,
				DirectX::TEX_COMPRESS_DEFAULT,
				threshold,
				*temporaryImage);

			if (SUCCEEDED(result)) {
				metadata = temporaryImage->GetMetadata();
				outputImage = std::move(temporaryImage);
			}
		} else if (metadata.format != dxgiFormat) {
			ImageWrapper imageWrapper(makeUNORM(outputImage->GetImage(0, 0, 0), outputImage->GetImageCount()));
			result = DirectX::Convert(
				imageWrapper.get(),
				outputImage->GetImageCount(),
				metadata,
				dxgiFormat,
				DirectX::TEX_FILTER_DEFAULT,
				threshold,
				*temporaryImage);

			if (SUCCEEDED(result)) {
				metadata = temporaryImage->GetMetadata();
				outputImage = std::move(temporaryImage);
				temporaryImage.reset(new DirectX::ScratchImage());
			}
		}
	}

	DirectX::Blob blob;
	if (SUCCEEDED(result)) {
		FileType fileType = static_cast<FileType>(settings->fileType);
		if (fileType == DDS) {
			result = DirectX::SaveToDDSMemory(
				outputImage->GetImages(),
				outputImage->GetImageCount(),
				metadata,
				DirectX::DDS_FLAGS_NONE,
				blob);
		} else if (fileType == WIC) {
			result = DirectX::SaveToWICMemory(
				outputImage->GetImages(),
				outputImage->GetImageCount(),
				DirectX::WIC_FLAGS_NONE,
				GUID_ContainerFormatPng,
				blob,
				&GUID_WICPixelFormat32bppBGRA);
		} else {
			result = S_FALSE;
		}
	}

	if (SUCCEEDED(result)) {
		::map(texture->metadata, metadata);
		texture->size = blob.GetBufferSize();
		texture->buffer = new unsigned char[texture->size];
		memcpy(texture->buffer, blob.GetBufferPointer(), texture->size);
	}

	return result;
}

HRESULT TextureService::getTextureMetadata(const char *texturePath, TextureMetadata *textureMetadata) {
	std::wstring systemImagePath = core::toWString(std::string(texturePath));

	DirectX::TexMetadata metadata;
	HRESULT result = DirectX::GetMetadataFromDDSFile(systemImagePath.c_str(), DirectX::DDS_FLAGS_NONE, metadata);

	if (SUCCEEDED(result)) {
		::map(*textureMetadata, metadata);
	}

	return result;
}

void TextureService::release(void *textureBuffer) {
	if (textureBuffer) {
		delete[] textureBuffer;
	}
}