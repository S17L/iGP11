#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
    class NoCodeFileResourceProvider : public core::IResourceProvider {
    public:
        virtual ~NoCodeFileResourceProvider() {}
        virtual std::string get(const std::string &key) override {
            throw core::exception::ResourceNotFoundException(core::stringFormat(ENCRYPT_STRING("disabled: %s"), key.c_str()));
        }
    };

    class RealCodeFileResourceProvider : public core::IResourceProvider {
    private:
        std::string _directoryPath;
    public:
        RealCodeFileResourceProvider(std::string directoryPath)
            : _directoryPath(directoryPath) {}
        virtual ~RealCodeFileResourceProvider() {}
        virtual std::string get(const std::string &key) override {
            auto filePath = core::file::combine(_directoryPath, key);
            std::ifstream file(filePath);
            if (file.fail()) {
                throw core::exception::ResourceNotFoundException(core::stringFormat(ENCRYPT_STRING("file: %s"), filePath.c_str()));
            }

            std::string fileContent((std::istreambuf_iterator<char>(file)), std::istreambuf_iterator<char>());
            return fileContent;
        }
    };

    class CachedCodeResourceProvider : public core::IResourceProvider {
    public:
        virtual ~CachedCodeResourceProvider() {}
        virtual std::string get(const std::string &key) override;
    };
}