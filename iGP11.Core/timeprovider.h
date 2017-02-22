#include "stdafx.h"
#include "igp11core.h"

namespace core {
	namespace time {
		class CurrentTimeProvider : public ITimeProvider {
		public:
			virtual ~CurrentTimeProvider() {}
			virtual DateTime getTime() override;
		};
	}
}