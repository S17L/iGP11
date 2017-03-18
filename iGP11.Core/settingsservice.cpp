#include "stdafx.h"
#include "settingsservice.h"

const std::string _filePath = ENCRYPT_STRING("iGP11\\settings.igp");

std::string getFileContent(const std::string &filePath) {
	std::ifstream file(filePath);
    if (file.fail()) {
        throw core::exception::ResourceNotFoundException(core::stringFormat(ENCRYPT_STRING("file: %s"), _filePath.c_str()));
    }

	std::string content;
	if (file) {
		std::getline(file, content);
		file.close();
	}

	return content;
}

core::dto::GameSettings core::SettingsService::getSettings() {
	return _serializer->deserializeSettings(::getFileContent(::_filePath));
}