#pragma once

//defines a logger for the CLR Host

#include <Util/String.hpp>

namespace RealLoader::Util
{
	//prints a message to the console
	static inline void LogMessage(const RealString& message)
	{
#if defined(_WIN32)
		wprintf(STR("%s\n"), message.data.c_str());
#else
		printf("%s\n", message.data.c_str());
#endif
	}

	//prints a error
	static inline void LogError(const RealString& type, const RealString& message)
	{
#if defined(_WIN32)
		wprintf(STR("Real Loader Error: %s || %s\n"), type.data.c_str(), message.data.c_str());
#else
		printf("Real Loader Error: %s || %s\n", type.data.c_str(), message.data.c_str());
#endif
	}

	//prints a fatal error
	static inline void LogFatalError(const RealString& type, const RealString& message)
	{
#if defined(_WIN32)
		wprintf(STR("Real Loader FATAL ERROR: %s || %s\n"), type.data.c_str(), message.data.c_str());
#else
		printf("Real Loader FATAL ERROR: %s || %s\n", type.data.c_str(), message.data.c_str());
#endif
	}
}