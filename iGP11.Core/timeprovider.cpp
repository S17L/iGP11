#include "stdafx.h"
#include "timeprovider.h"

core::time::DateTime core::time::CurrentTimeProvider::getTime() {
	core::time::DateTime dateTime;
	SYSTEMTIME time;
	GetSystemTime(&time);

	dateTime.year = time.wYear;
	dateTime.month = time.wMonth;
	dateTime.day = time.wDay;
	dateTime.hour = time.wHour;
	dateTime.minute = time.wMinute;
	dateTime.second = time.wSecond;
	dateTime.milliseconds = time.wMilliseconds;
	dateTime.totalMiliseconds = std::chrono::system_clock::now().time_since_epoch().count() / 10000;

	return dateTime;
}