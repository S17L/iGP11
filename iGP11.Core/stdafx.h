#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN

#include <ctype.h>
#include <stdio.h>
#include <tchar.h>
#include <Windows.h>
#include <psapi.h>
#include <winsock2.h>
#include <TlHelp32.h>

#include <algorithm>
#include <cctype>
#include <chrono>
#include <cmath>
#include <codecvt>
#include <ctime>
#include <exception>
#include <fstream>
#include <functional>
#include <iomanip>
#include <iostream>
#include <iterator>
#include <list>
#include <locale>
#include <map>
#include <memory>
#include <mutex>
#include <sstream>
#include <streambuf>
#include <string>
#include <thread>
#include <vector>
#include <utility>

#define SAFE_RELEASE(p) { if ( (p) ) { (p)->Release(); (p) = 0; } }
#define SAFE_RELEASE_ARRAY(pointer, i) { if (pointer != nullptr && pointer[i] != nullptr) { pointer[i]->Release(); pointer[i] = 0; } }
#define SAFE_DELETE(a) if( (a) != NULL ) delete (a); (a) = NULL;

typedef unsigned __int64 uint64_t;