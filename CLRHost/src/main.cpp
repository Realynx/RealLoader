#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files
#include <windows.h>

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

//defines a struct for passing parameters
struct CLRHostRun_Params
{
	CLR::CLRHost* host = nullptr;
	std::basic_string<char_t> app_path;
};

//the main bulk of code for the actually running of DLLs and such
int LoadCLRHost(CLRHostRun_Params* params)
{
	//const std::basic_string<char_t> app_path = root_path + STR("ManagedModFramework\\PalworldManagedModFramework.dll");

	//init the function pointers
	auto configPath = STR("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld\\Pal\\Binaries\\Win64\\ManagedModFramework\\PalworldManagedModFramework.runtimeconfig.json");
	if (!params->host->Init(configPath))
	{
		std::cout << "Failed To Init ^2" << std::endl;
		return 1;
	}

	std::cout << "Init ClrHost Done" << std::endl;

	params->host->StartAssembly(params->app_path.c_str());
	std::cout << "After Start Assembly" << std::endl;

	return 0;
}

//runs the CLR
void RUNCLR()
{
	CLR::CLRHost host;
	CLRHostRun_Params runParams;
	runParams.host = &host;
	runParams.app_path = L"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld\\Pal\\Binaries\\Win64\\ManagedModFramework\\PalworldManagedModFramework.dll";

	LoadCLRHost(&runParams);
}

void SpawnClrThread() {
	AllocConsole();
	freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);
	std::cout << "Injected" << std::endl;

	std::thread worker(RUNCLR);
	worker.detach();
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  reason, LPVOID lpReserved)
{

	switch (reason)
	{
	case DLL_PROCESS_ATTACH:
		SpawnClrThread();
		break;

	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;

	case DLL_PROCESS_DETACH:
		FreeConsole();
		break;
	}


	return TRUE;
}