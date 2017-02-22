#include "stdafx.h"
#include "texturecache.h"

const std::string delimiter = ENCRYPT_STRING(", ");
const std::string nullValue = ENCRYPT_STRING("NULL");

const std::string keyMapFrom = ENCRYPT_STRING("map_from");
const std::string keyForceSrgb = ENCRYPT_STRING("force_srgb");
const std::string keyMapTo = ENCRYPT_STRING("map_to");

std::vector<std::shared_ptr<core::IValueMapper>> _mappers = {
	std::shared_ptr<core::IValueMapper>(new core::BoolValueMapper(core::stringFormat(ENCRYPT_STRING("%s="), keyForceSrgb.c_str()), [](core::TextureProfile &profile, bool value) { profile.forceSrgb.set(value); })),
	std::shared_ptr<core::IValueMapper>(new core::TextValueMapper(core::stringFormat(ENCRYPT_STRING("%s="), keyMapTo.c_str()), [](core::TextureProfile &profile, std::string value) { profile.mapTo.set(value); }))
};

void applyProfile(core::TextureProfile &profile, std::list<std::string> &propertyRows) {
	for (auto propertyRow : propertyRows) {
		core::trim(propertyRow);
		core::toLower(propertyRow);
		for (auto mapper : _mappers) {
			if (mapper->isApplicable(propertyRow)) {
				mapper->apply(profile, propertyRow);
				break;
			}
		}
	}
}

bool core::BoolValueMapper::isApplicable(const std::string &propertyRow) {
	return core::containsStartAt(propertyRow, _key);
}

void core::BoolValueMapper::apply(core::TextureProfile &profile, const std::string &propertyRow) {
	auto intOption = ENCRYPT_STRING("1");
	auto boolOption = ENCRYPT_STRING("true");
	std::string value = propertyRow.substr(_key.size());
	if (!value.empty() && (core::isEqual(value, intOption) || core::isEqual(value, boolOption))) {
		_mappingAction(profile, true);
	}
}

bool core::TextValueMapper::isApplicable(const std::string &propertyRow) {
	return core::containsStartAt(propertyRow, _key);
}

void core::TextValueMapper::apply(core::TextureProfile &profile, const std::string &propertyRow) {
	auto value = propertyRow.substr(_key.size());
	if (!value.empty()) {
		_mappingAction(profile, value);
	}
}

void core::StateTextureCacheVisitor::visit(const core::TextureProfile &profile) {
	auto forceSrgb = profile.forceSrgb;
	auto mapTo = profile.mapTo;

	_profiles.push_back(core::stringFormat(
        ENCRYPT_STRING("[ %s: %s, %s: %s, %s: %s ]"),
		keyMapFrom.c_str(),
		profile.mapFrom.c_str(),
		keyForceSrgb.c_str(),
		(forceSrgb.hasValue() ? (core::stringFormat(ENCRYPT_STRING("%d"), forceSrgb.get())) : nullValue).c_str(),
		keyMapTo.c_str(),
		(mapTo.hasValue() ? mapTo.get() : nullValue).c_str()));
}

std::string core::StateTextureCacheVisitor::build() {
	return !_profiles.empty()
		? core::join(delimiter, _profiles)
		: nullValue;
}

core::TextureCache::TextureCache(std::string directoryPath) {
	cache(directoryPath);
}

size_t core::TextureCache::getCount() {
	return _profiles.size();
}

bool core::TextureCache::has(const std::string &id) {
	for (auto profile : _profiles) {
		if (profile->isFor(id)) {
			return true;
		}
	}

	return false;
}

void core::TextureCache::accept(core::ITextureCacheVisitor &visitor) {
	for (auto profile : _profiles) {
		visitor.visit(*profile.get());
	}
}

std::shared_ptr<core::TextureProfile> core::TextureCache::find(const std::string &id) {
	for (auto mappingFile : _profiles) {
		if (mappingFile->isFor(id)) {
			return mappingFile;
		}
	}

	return nullptr;
}

void core::TextureCache::merge(core::TextureProfile &profile) {
	auto existingProfile = find(profile.mapFrom);
	if (existingProfile != nullptr) {
		if (existingProfile->forceSrgb.hasValue()) {
			profile.forceSrgb.set(existingProfile->forceSrgb);
		}
		if (existingProfile->mapTo.hasValue()) {
			profile.mapTo.set(existingProfile->mapTo);
		}
	}
}

std::list<std::string> core::TextureCache::getTextures(const std::string &directoryPath) {
	WIN32_FIND_DATA findData;
	std::list<std::string> output;
	auto search = core::toWString(core::file::combine(directoryPath, ENCRYPT_STRING("*.dds")));
	auto handle = ::FindFirstFile(search.c_str(), &findData);

	if (handle != INVALID_HANDLE_VALUE) {
		do {
			if (!(findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
				auto fileName = core::toString(std::wstring(findData.cFileName));
				auto fileNameWithoutExtension = fileName.substr(0, fileName.find_last_of("."));

				output.push_back(fileNameWithoutExtension);
			}
		} while (::FindNextFile(handle, &findData));
		::FindClose(handle);
	}

	return output;
}

std::map<std::string, std::list<std::string>> core::TextureCache::getTextureProfiles(const std::string &directoryPath) {
	WIN32_FIND_DATA findData;
	std::map<std::string, std::list<std::string>> output;
	auto search = core::toWString(core::file::combine(directoryPath, ENCRYPT_STRING("*.txt")));
	auto handle = ::FindFirstFile(search.c_str(), &findData);

	if (handle != INVALID_HANDLE_VALUE) {
		do {
			if (!(findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
				auto fileName = core::toString(std::wstring(findData.cFileName));
				auto fileNameWithoutExtension = fileName.substr(0, fileName.find_last_of("."));
				auto fileAbsolutePath = core::file::combine(directoryPath, fileName);
				std::ifstream file(fileAbsolutePath);

				std::string propertyRow;
				std::list<std::string> propertyRows;

				if (file) {
					while (std::getline(file, propertyRow)) {
						propertyRows.push_back(propertyRow);
					}

					file.close();
				}

				output.insert(std::pair<std::string, std::list<std::string>>(fileNameWithoutExtension, propertyRows));
			}
		} while (::FindNextFile(handle, &findData));
		::FindClose(handle);
	}

	return output;
}

void core::TextureCache::cache(const std::string &directoryPath) {
	auto textures = getTextures(directoryPath);
	auto textureProfiles = getTextureProfiles(directoryPath);

	for (auto texture : textures) {
		std::shared_ptr<core::TextureProfile> profile(new core::TextureProfile());
		profile->mapFrom = texture;
		auto rawProfile = textureProfiles.find(texture);
		if (rawProfile != textureProfiles.end()) {
			::applyProfile(*profile.get(), rawProfile->second);
			textureProfiles.erase(rawProfile);
		}

		_profiles.push_back(profile);
	}

	for (auto rawProfile : textureProfiles) {
		std::shared_ptr<core::TextureProfile> profile(new core::TextureProfile());
		profile->mapFrom = rawProfile.first;
		::applyProfile(*profile.get(), rawProfile.second);
		_profiles.push_back(profile);
	}
}

std::shared_ptr<core::ITextureCache> core::TextureCacheFactory::createFromDirectory(std::string directoryPath) {
	return std::shared_ptr<core::ITextureCache>(new core::TextureCache(directoryPath));
}