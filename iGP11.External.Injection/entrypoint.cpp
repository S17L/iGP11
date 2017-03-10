#include "stdafx.h"
#include "igp11core.h"
#include "processservice.h"

#define DLL __declspec(dllexport)

std::unique_ptr<core::IProcessService> _processService;

#ifdef __cplusplus
extern "C"
{
#endif

    DLL bool getProcessFilePath(unsigned long id, char *filePath, unsigned long maxLength) {
        auto detail = _processService->getProcessDetail(id);
        if (!detail.path.empty()) {
            auto size = min(maxLength - 1, detail.path.size());
            strncpy(filePath, _processService->getProcessDetail(id).path.c_str(), size);
            filePath[size] = '\0';

            return true;
        }

        return false;
    }

    DLL unsigned long inject(
        const char *applicationFilePath,
        const char *proxyFilePath) {
        return _processService->inject(
            std::string(applicationFilePath),
            std::string(proxyFilePath));
    }

	DLL bool isProcessRunning(const char *applicationFilePath) {
		return _processService->getProcessByName(std::string(applicationFilePath)) > 0;
	}

	DLL bool isProxyLoaded(
		const char *applicationFilePath,
		const char *proxyFilePath) {
		return _processService->hasLoadedLibrary(
			std::string(applicationFilePath),
			std::string(proxyFilePath));
	}

#ifdef __cplusplus
}
#endif

BOOL APIENTRY DllMain(HINSTANCE module, DWORD reason, LPVOID reserved) {
	if (reason == DLL_PROCESS_ATTACH) {
		_processService.reset(new core::ProcessService());
		_processService->adjustPrivileges();
	} else if (reason == DLL_PROCESS_DETACH) {
		_processService.reset();
	}

	return TRUE;
}

