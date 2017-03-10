#include "stdafx.h"
#include "processservice.h"

int core::ProcessService::adjustPrivileges() {
	HANDLE token;
	TOKEN_PRIVILEGES tokenPrivileges;
	if (OpenProcessToken(::GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &token)) {
		LookupPrivilegeValue(NULL, SE_DEBUG_NAME, &tokenPrivileges.Privileges[0].Luid);
		tokenPrivileges.PrivilegeCount = 1;
		tokenPrivileges.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
		if (AdjustTokenPrivileges(token, 0, &tokenPrivileges, sizeof(tokenPrivileges), NULL, NULL) == 0) {
			return 1;
		} else {
			return 0;
		}
	}

	return 1;
}

core::dto::ProcessDetail core::ProcessService::getCurrentProcessDetail() {
	return getProcessDetail(::GetCurrentProcessId());
}

core::dto::ProcessDetail core::ProcessService::getProcessDetail(unsigned long id) {
    core::dto::ProcessDetail detail;
    detail.id = id;

    TCHAR path[MAX_PATH];
    core::disposing::unique_ptr<void> process = core::disposing::makeHandle(::OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, id));

    if (process.get()) {
        if (GetModuleFileNameEx(process.get(), NULL, path, MAX_PATH) != 0) {
            detail.path = core::toString(path);
        }
    }

    return detail;
}

unsigned long core::ProcessService::getProcessByName(const std::string &applicationFilePath) {
	core::disposing::unique_ptr<void> snapshot = core::disposing::makeHandle(::CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0));
	PROCESSENTRY32 process;
	ZeroMemory(&process, sizeof(process));
	process.dwSize = sizeof(process);

	if (Process32First(snapshot.get(), &process)) {
		do {
			core::dto::ProcessDetail detail = getProcessDetail(process.th32ProcessID);
			if (!detail.path.empty() && core::isEqualIgnoreCase(detail.path, applicationFilePath)) {
				return process.th32ProcessID;
			}
		} while (Process32Next(snapshot.get(), &process));
	}

	return 0;
}

bool core::ProcessService::hasLoadedLibrary(const std::string &applicationFilePath, const std::string &libraryFilePath) {
	unsigned long pid = getProcessByName(applicationFilePath);
	if (pid > 0) {
		core::disposing::unique_ptr<void> process = core::disposing::makeHandle(::OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, pid));

		unsigned long size;
		HMODULE modules[1024];

		if (::EnumProcessModules(process.get(), modules, sizeof(modules), &size)) {
			for (unsigned int i = 0; i < (size / sizeof(HMODULE)); i++) {
				TCHAR path[MAX_PATH];
				int j = GetModuleFileNameEx(process.get(), modules[i], path, MAX_PATH);
				if (j != 0 && core::isEqualIgnoreCase(core::toString(path), libraryFilePath)) {
					return true;
				}
			}
		}
	}

	return false;
}

unsigned long core::ProcessService::inject(const std::string &applicationFilePath, const std::string &libraryFilePath) {
	char dllFullName[MAX_PATH];
	::strcpy_s(dllFullName, libraryFilePath.c_str());

	auto pid = getProcessByName(applicationFilePath);
	if (pid <= 0) {
		return 11860;
	}

	auto process = ::OpenProcess(
		PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_CREATE_THREAD,
		FALSE,
		pid);

	if (!process) {
		return GetLastError();
	}

	auto memorySize = strlen(dllFullName) + 1;
	void *address = ::VirtualAllocEx(
		process,
		NULL,
		memorySize,
		MEM_COMMIT | MEM_RESERVE,
		PAGE_EXECUTE_READWRITE);

	if (!address) {
		return 11861;
	}

	if (!::WriteProcessMemory(
		process,
		address,
		(void*)dllFullName,
		memorySize,
		NULL)) {
		return 11862;
	}

	HMODULE kernel32 = ::GetModuleHandle(L"Kernel32");
	if (!kernel32) {
		return 11863;
	}

	auto thread = ::CreateRemoteThread(
		process,
		NULL,
		0,
		(LPTHREAD_START_ROUTINE) ::GetProcAddress(kernel32, ENCRYPT_STRING("LoadLibraryA")),
		address,
		0,
		NULL);

	if (!thread) {
		return 11864;
	}

	auto waitingResult = ::WaitForSingleObject(
		thread,
		INFINITE);

	if (waitingResult != WAIT_OBJECT_0) {
		return waitingResult;
	}

	DWORD exitCode;
	if (!::GetExitCodeThread(
		thread,
		&exitCode)) {
		return 11865;
	}

	::VirtualFreeEx(
		process,
		address,
		0,
		MEM_RELEASE);

	if (!::CloseHandle(thread)) {
		return 11866;
	}

	if (!::CloseHandle(process)) {
		return 11867;
	}

	return 0;
}