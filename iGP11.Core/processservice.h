#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	class ProcessService : public IProcessService {
	public:
		~ProcessService() {}
		virtual int adjustPrivileges() override;
		virtual dto::ProcessDetail getCurrentProcessDetail() override;
        virtual dto::ProcessDetail getProcessDetail(unsigned long id) override;
		virtual unsigned long getProcessByName(const std::string &applicationFilePath) override;
		virtual bool hasLoadedLibrary(const std::string &applicationFilePath, const std::string &libraryFilePath) override;
		virtual unsigned long inject(const std::string &applicationFilePath, const std::string &libraryFilePath) override;
	};
}