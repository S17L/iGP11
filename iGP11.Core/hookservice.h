#include "stdafx.h"
#include "igp11core.h"

namespace core {
	struct Target {
		std::string identifier;
		LPVOID address;
	};

	class MinHookTransaction : public IHookTransaction {
	private:
		std::list<Target> _targets;
		bool _hasError;
		bool _isFinished;
		bool enable();
	public:
		MinHookTransaction()
			: _hasError(false), _isFinished(false) {};
		virtual ~MinHookTransaction();
		virtual bool commit() override;
		virtual void hook(const std::string &identifier, LPVOID target, LPVOID detour, LPVOID *original) override;
	};

	class MinHookService : public IHookService {
		friend class MinHookTransaction;
	private:
		bool _initialized;
		std::mutex _mutex;
		std::list<Target> _targets;
		MinHookService();
	public:
		static MinHookService& getInstance() {
			static MinHookService instance;
			return instance;
		}
		virtual ~MinHookService();
		MinHookService(MinHookService const&) = delete;
		void operator=(MinHookService const&) = delete;
		virtual std::unique_ptr<IHookTransaction> openTransaction() override;
		virtual bool unhookAll() override;
	};
}