#include "stdafx.h"
#include "igp11core.h"

#include <sys/stat.h>

std::string core::toString(std::wstring input) {
	if (input.empty()) {
		return std::string();
	}

	int length = ::WideCharToMultiByte(CP_UTF8, 0, &input[0], (int)input.size(), NULL, 0, NULL, NULL);
	std::string output(length, 0);
	::WideCharToMultiByte(CP_UTF8, 0, &input[0], (int)input.size(), &output[0], length, NULL, NULL);

	return output;
}

std::wstring core::toWString(std::string input) {
	if (input.empty()) {
		return std::wstring();
	}

	int length = ::MultiByteToWideChar(CP_UTF8, 0, &input[0], (int)input.size(), NULL, 0);
	std::wstring output(length, 0);
	::MultiByteToWideChar(CP_UTF8, 0, &input[0], (int)input.size(), &output[0], length);

	return output;
}

float core::algorithm::gaussian(float x, float sigma) {
	return float(1.0 / sqrt(2 * (float)M_PI * pow(sigma, 2))) * exp(-pow(x, 2) / (2 * pow(sigma, 2)));
}

uint64_t core::algorithm::hash64(const void *key, unsigned int length, unsigned int resolution, unsigned int seed) {
	if (key == nullptr || length == 0 || seed == 0) {
		return 0;
	}

	const unsigned int resolutionLength = length / 8;
	const unsigned int increment = max((unsigned int)1, resolutionLength / resolution);
	const unsigned int stepsCount = resolutionLength / increment;
	const unsigned int remainder = resolutionLength % increment;

	const uint64_t m = 0xc6a4a7935bd1e995;
	const int r = 47;

	uint64_t h = seed ^ (length * m);

	const uint64_t *data = (const uint64_t*)key;
	for (unsigned int i = 0; i < stepsCount; i++) {
		uint64_t k = *data;
		k *= m;
		k ^= k >> r;
		k *= m;
		h ^= k;
		h *= m;
		data += increment;
	}

	data += remainder;
	const unsigned char *last = (const unsigned char*)data;

	switch (length & 7) {
	case 7: h ^= uint64_t(last[6]) << 48;
	case 6: h ^= uint64_t(last[5]) << 40;
	case 5: h ^= uint64_t(last[4]) << 32;
	case 4: h ^= uint64_t(last[3]) << 24;
	case 3: h ^= uint64_t(last[2]) << 16;
	case 2: h ^= uint64_t(last[1]) << 8;
	case 1: h ^= uint64_t(last[0]);
		h *= m;
	};

	h ^= h >> r;
	h *= m;
	h ^= h >> r;

	return h;
}

std::string core::file::addTrailingSlash(const std::string &path) {
	if (core::endsWith(path, ENCRYPT_STRING("/\\"))) {
		return path;
	} else {
		return core::stringFormat(ENCRYPT_STRING("%s\\"), path.c_str());
	}
}

std::string core::file::combine(const std::string &first, const std::string &second) {
	return core::stringFormat(ENCRYPT_STRING("%s%s"), core::file::addTrailingSlash(first).c_str(), core::file::removeLeadingSlash(second).c_str());
}

bool core::file::fileExists(const std::string &name) {
	struct stat buffer;
	return (stat(name.c_str(), &buffer) == 0);
}

std::string core::file::getDirectory(const std::string &path) {
	size_t found = path.find_last_of(ENCRYPT_STRING("/\\"));
	return path.substr(0, found);
}

void core::file::readToEnd(std::string filePath, std::string &fileContent) {
	std::ifstream file(filePath.c_str());
	file.seekg(0, std::ios::end);
	fileContent.reserve(file.tellg());
	file.seekg(0, std::ios::beg);
	fileContent.assign((std::istreambuf_iterator<char>(file)), std::istreambuf_iterator<char>());
}

std::string core::file::removeLeadingSlash(const std::string &path) {
	if (core::startsWith(path, ENCRYPT_STRING("/\\"))) {
		return path.substr(2);
	} else {
		return path;
	}
}

std::unique_ptr<core::logging::ILogger> core::logging::Logger::current = std::unique_ptr<core::logging::ILogger>(new core::logging::FakeLogger());

core::logging::ThreadLogTextAppender::ThreadLogTextAppender(std::string prefix) {
    _id = std::this_thread::get_id();
    _prefix = prefix;
}

void core::logging::ThreadLogTextAppender::append(std::string &text) {
    if (_id == std::this_thread::get_id()) {
        text.append(_prefix);
    }
}

std::string core::time::toString(const core::time::DateTime &time) {
	std::stringstream stream;
	stream << time.year << "-"
		<< std::setw(2) << std::setfill('0') << time.month << '-'
		<< std::setw(2) << std::setfill('0') << time.day << ' '
		<< std::setw(2) << std::setfill('0') << time.hour << ':'
		<< std::setw(2) << std::setfill('0') << time.minute << ':'
		<< std::setw(2) << std::setfill('0') << time.second << '.'
		<< std::setw(3) << std::setfill('0') << time.milliseconds;

	return stream.str();
}