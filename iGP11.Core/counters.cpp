#include "stdafx.h"
#include "counters.h"

unsigned int core::FrameCounter::getAverageCount() const {
	return _averageCount;
}

unsigned int core::FrameCounter::getTotalCount() const {
	return _totalCount;
}

bool core::FrameCounter::nextFrame() {
	_counter++;
	_totalCount++;

	if (_time == 0) {
		_time = _timeProvider->getTime().totalMiliseconds;
	}

	auto currentTime = _timeProvider->getTime().totalMiliseconds;
	auto difference = currentTime - _time;

	if (difference >= 1000) {
		_time = currentTime;
		_averageCount = static_cast<unsigned int>(::ceil(1000 * (float)_counter / difference));
		_counter = 0;

		return true;
	}

	return false;
}