// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Standard headers
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

#include <CLR/CLR.hpp>

using string_t = std::basic_string<char_t>;

static inline void CLRRun(CLR::CLRHost* host, const string_t& app_path)
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
int run_app_example(CLR::CLRHost* host, const string_t& root_path)
{
	const string_t app_path = root_path + STR("ManagedModFramework\\PalworldManagedModFramework.dll");

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

//the entry point
#if defined(Window_Build)
int __cdecl wmain(int argc, wchar_t* argv[])
#else
int main(int argc, char* argv[])
#endif
{
	//get args if they exist
	std::string path = "", modFlag = "-modded";
	if (argc > 0)
	{
		//const wchar_t* a = (wchar_t*)"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld\\Pal\\Binaries\\Win64\\Palworld-Win64-Shipping.exe";
		path = std::string(CStringA(argv[0]).GetString());
	}
	//printf(""path.c_str());

	// gets the path of the real game, and runs it with the needed arguments
	std::filesystem::path absolutePath = path;
	absolutePath = absolutePath.replace_filename("Game-Palworld-Win64-Shipping").string() + ".exe";

	std::string command = std::string("\"") + absolutePath.string() + std::string("\"");

	//the CLR Host
	CLR::CLRHost host;

	//if mods are enabled
	bool isModded = !strcmp(modFlag.c_str(), "-modded");
	if (isModded)
	{
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

		run_app_example(&host, root_path);
	}

	system(command.c_str()); //execute the game

	if (isModded) //hold console if the game is modded
	{
		
		getchar();
	}

	return 0;
}