// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Standard headers
#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <iostream>
#include <thread>
#include <vector>

#include <CLR/CLR.hpp>

using string_t = std::basic_string<char_t>;

//the main bulk of code for the actually running of DLLs and such
int run_app_example(CLR::CLRHost* host, const string_t& root_path)
{
	const string_t app_path = root_path + STR("ManagedModFramework") + DIR_SEPARATOR + STR("PalworldManagedModFramework.dll");

	//init the function pointers
	if (!host->Init(app_path.c_str()))
	{
		assert(false && "Failure: load_hostfxr()");
		return EXIT_FAILURE;
	}

	// Load .NET Core
	hostfxr_handle cxt = nullptr;
	std::vector<const char_t*> args{ app_path.c_str(), STR("app_arg_1"), STR("app_arg_2") };
	int rc = host->init_for_cmd_line_fptr(args.size(), args.data(), nullptr, &cxt);
	if (rc != 0 || cxt == nullptr)
	{
		std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
		host->close_fptr(cxt);
		return EXIT_FAILURE;
	}

	//-----C++ CODE HERE------//

	//-----C++ CODE HERE------//

	// Run the app
	host->run_app_fptr(cxt);

	//host.close_fptr(cxt);
	return EXIT_SUCCESS;
}

//the entry point
#if defined(Window_Build)
int __cdecl wmain(int argc, wchar_t* argv[])
#else
int main(int argc, char* argv[])
#endif
{
	CLR::CLRHost host;

	// Get the current executable's directory
	// This sample assumes the managed assembly to load and its runtime configuration file are next to the host
	char_t host_path[MAX_PATH];
#if Window_Build
	auto size = ::GetFullPathNameW(argv[0], sizeof(host_path) / sizeof(char_t), host_path, nullptr);
	assert(size != 0);
#else
	auto resolved = realpath(argv[0], host_path);
	assert(resolved != nullptr);
#endif

	string_t root_path = host_path;
	auto pos = root_path.find_last_of(DIR_SEPARATOR);
	assert(pos != string_t::npos);
	root_path = root_path.substr(0, pos + 1);

	//set up .Net
	run_app_example(&host, root_path);

	getchar();
	return 0;
}