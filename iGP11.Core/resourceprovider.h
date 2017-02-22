#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	class FileResourceProvider : public core::IResourceProvider {
	public:
		virtual ~FileResourceProvider() {}
		virtual std::string get(const std::string &key) override;
	};
}