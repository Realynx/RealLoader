
#include <Windows.h>
#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <iostream>
#include <vector>
#include <filesystem>
#include "atlbase.h"
#include "atlstr.h"
#include "comutil.h"
#include <thread>

#include "CLR.hpp"

static inline void CLRRun(CLR::CLRHost* host, const std::basic_string<char_t>& app_path)
{
	std::this_thread::sleep_for(std::chrono::seconds(2));

	// Load .NET Core
	std::vector<const char_t*> args{ app_path.c_str() };
	int rc = host->init_for_cmd_line_fptr(args.size(), args.data(), nullptr, &host->cxt);

	//run the CLR
	host->run_app_fptr(host->cxt);

	//clean up CLR when done
	host->close_fptr(host->cxt);
}

//the main bulk of code for the actually running of DLLs and such
int run_app_example(CLR::CLRHost* host, const std::basic_string<char_t>& root_path)
{
	const std::basic_string<char_t> app_path = root_path + STR("ManagedModFramework\\PalworldManagedModFramework.dll");

	//init the function pointers
	if (!host->Init(app_path.c_str()))
	{
		assert(false && "Failure: load_hostfxr()");
		return EXIT_FAILURE;
	}

	// Load .NET Core
	//std::vector<const char_t*> args{ app_path.c_str() };
	//int rc = host->init_for_cmd_line_fptr(args.size(), args.data(), nullptr, &host->cxt);
	//if (rc != 0 || host->cxt == nullptr)
	//{
	//	std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
	//	host->close_fptr(host->cxt);
	//	return EXIT_FAILURE;
	//}

	// Create and start the thread
	std::thread clrThread(CLRRun, host, app_path);

	clrThread.detach();

	//host->run_app_fptr(host->cxt);


	return EXIT_SUCCESS;
}



__declspec(dllexport) void Load()
{
	// Get the current executable's directory
	// This sample assumes the managed assembly to load and its runtime configuration file are next to the host
//	char_t host_path[MAX_PATH];
//#if Window_Build
//	auto size = ::GetFullPathNameW(argv[0], sizeof(host_path) / sizeof(char_t), host_path, nullptr);
//	assert(size != 0);
//#else
//	auto resolved = realpath("ManagedModFramework\\", host_path);
//	assert(resolved != nullptr);
//#endif

	//load the DLL into the CLR Host
	CLR::CLRHost host;
	std::basic_string<char_t> root_path = L"ManagedModFramework\\PalworldManagedModFramework.dll";
	auto pos = root_path.find_last_of(DIR_SEPARATOR);
	assert(pos != std::basic_string<char_t>::npos);
	root_path = root_path.substr(0, pos + 1);

	run_app_example(&host, root_path);
}