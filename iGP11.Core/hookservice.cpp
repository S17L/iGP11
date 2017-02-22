#include "stdafx.h"
#include "hookservice.h"

#include "MinHook.h"

#define MIN_HOOK_MAX_PERFORMANCE 1

using namespace core::logging;

bool core::MinHookTransaction::enable() {
    bool hasError = false;

#if MIN_HOOK_MAX_PERFORMANCE == 1
    log(debug, ENCRYPT_STRING("attempting to enable all hooks at once..."));
    if (::MH_EnableHook(MH_ALL_HOOKS) == MH_OK) {
        log(debug, ENCRYPT_STRING("all hooks were enabled"));
    }
    else {
        log(debug, ENCRYPT_STRING("all hooks were not enabled... performing rollback"));
        hasError = true;
    }

    if (hasError) {
        auto result = ::MH_DisableHook(MH_ALL_HOOKS) == MH_OK;
        log(debug, ENCRYPT_STRING("disabled previously enabled hooks"), result);
    }
    else {
        core::MinHookService &hookService = core::MinHookService::getInstance();
        for (auto target : _targets) {
            hookService._targets.push_front(target);
        }
    }
#else
    std::mutex mutex;
    std::list<core::Target> enabledTargets;
    std::list<std::thread> threads;

    for (auto target : _targets) {
        threads.push_back(std::thread([&enabledTargets, &hasError, &mutex, target]() {
            log(debug, core::stringFormat(ENCRYPT_STRING("attempting to enable hook for %s[%p]..."), target.identifier.c_str(), target.address));
            if (::MH_EnableHook(target.address) == MH_OK) {
                log(debug, core::stringFormat(ENCRYPT_STRING("hook for %s was enabled"), target.identifier.c_str()));
                std::lock_guard<std::mutex> lock(mutex);
                enabledTargets.push_front(target);
            }
            else {
                log(error, core::stringFormat(ENCRYPT_STRING("hook for %s was not be enabled... performing rollback"), target.identifier.c_str()));
                hasError = true;
            }
        }));
    }

    for (std::thread &thread : threads) {
        thread.join();
    }

    if (hasError) {
    	for (auto target : enabledTargets) {
    		auto result = ::MH_DisableHook(target.address) == MH_OK;
    		log(debug, core::stringFormat(ENCRYPT_STRING("disabled previously enabled hook for: %s[%p]"), target.identifier.c_str(), target.address), result);
    	}
    } else {
    	core::MinHookService &hookService = core::MinHookService::getInstance();
    	for (auto target : _targets) {
    		hookService._targets.push_front(target);
    	}
    }
#endif

    return hasError;
}

core::MinHookTransaction::~MinHookTransaction() {
    MinHookService::getInstance()._mutex.unlock();
}

bool core::MinHookTransaction::commit() {
    if (_isFinished) {
        return false;
    }

    bool hasError = false;
    __try {
        hasError = enable();
    }
    __finally {
        _isFinished = true;
    }

    return !hasError;
}

void core::MinHookTransaction::hook(const std::string &identifier, LPVOID target, LPVOID detour, LPVOID *original) {
    if (_hasError || _isFinished) {
        return;
    }

    auto result = ::MH_CreateHook(target, detour, original) == MH_OK;
    _hasError |= !result;

    log(debug, core::stringFormat(ENCRYPT_STRING("created inactive hook for: %s"), identifier.c_str()), result);

    if (!_hasError) {
        core::Target container;
        container.identifier = identifier;
        container.address = target;
        _targets.push_back(container);
    }
}

core::MinHookService::MinHookService() {
    _initialized = ::MH_Initialize() == MH_OK;
}

core::MinHookService::~MinHookService() {
    if (!_initialized) {
        return;
    }

    ::MH_Uninitialize();
    _initialized = false;
}

std::unique_ptr<core::IHookTransaction> core::MinHookService::openTransaction() {
    _mutex.lock();
    return std::unique_ptr<core::IHookTransaction>(new MinHookTransaction());
}

bool core::MinHookService::unhookAll() {
    return true;
}