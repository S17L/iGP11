#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	class FrameCounter : public IFrameCounter {
	private:
		core::time::ITimeProvider* _timeProvider;
		long long _time;
		unsigned int _averageCount;
		unsigned int _totalCount;
		unsigned int _counter;
	public:
		FrameCounter(core::time::ITimeProvider *timeProvider)
			: _timeProvider(timeProvider), _averageCount(0), _totalCount(0), _counter(0), _time(0) {}
		virtual ~FrameCounter() {}
		virtual unsigned int getAverageCount() const override;
		virtual unsigned int getTotalCount() const override;
		virtual bool nextFrame() override;
	};
}