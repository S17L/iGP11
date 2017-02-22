#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	namespace communication {
		class RequestHandler : public IRequestHandler {
		private:
			core::ISerializer *_serializer;
			std::list<std::shared_ptr<ICommandHandlingPolicy>> _policies;
		public:
			RequestHandler(core::ISerializer *serializer)
				: _serializer(serializer) {}
			virtual ~RequestHandler() {}
			void addPolicy(std::shared_ptr<ICommandHandlingPolicy> policy);
			virtual std::string handle(const std::string &input) override;
		};
	}
}