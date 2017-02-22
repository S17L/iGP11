#include "stdafx.h"
#include "requesthandler.h"

void core::communication::RequestHandler::addPolicy(std::shared_ptr<core::communication::ICommandHandlingPolicy> policy) {
	_policies.push_back(policy);
}

std::string core::communication::RequestHandler::handle(const std::string &input) {
	auto command = _serializer->deserializeCommand(input);
	auto policies = core::linq::makeEnumerable(_policies)
		.where([&](const std::shared_ptr<core::communication::ICommandHandlingPolicy> &policy)->bool { return policy->isAppliciable(command.id); })
		.toVector();

	return !policies.empty()
		? policies.at(0)->handle(command.data)
		: std::string();
}