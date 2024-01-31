/*
DLL entry point for running C#
*/

#include "CLR.hpp"

#include <thread>
#include <filesystem>

//runs the CLR thread and DLL
void RUNCLR()
{
#if defined(_WIN32)
	std::basic_string<char_t> appPath = L"Pal\\Binaries\\Win64\\ManagedModFramework\\PalworldManagedModFramework.dll";
	auto configPath = STR("Pal\\Binaries\\Win64\\ManagedModFramework\\PalworldManagedModFramework.runtimeconfig.json");
#elif defined(__linux__)
	std::basic_string<char_t> appPath = L"Pal\\Binaries\\Linux\\ManagedModFramework\\PalworldManagedModFramework.dll";
	auto configPath = STR("Pal\\Binaries\\Linux\\ManagedModFramework\\PalworldManagedModFramework.runtimeconfig.json");
#endif

	auto fullAppPath = std::filesystem::absolute(appPath).wstring();
	auto fullConfigPath = std::filesystem::absolute(configPath).wstring();

	//init the CLR
	CLR::CLRHost host;
	if (!host.Init(fullConfigPath.c_str())) {
		std::cout << "Failed To Init Host" << std::endl;
		return;
	}

	std::cout << "Finished Initializing CLR" << std::endl;

	//starts the main Assembly
	host.StartAssembly(fullAppPath.c_str());
	}

void SpawnClrThread() {

	//inits console if running on windows.
#if defined(_WIN32)
	AllocConsole();
	freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);
#endif

	//starts thread and detachtes it
	std::thread worker(RUNCLR);
	worker.detach();
}


#if defined(_WIN32)
// Windows-specific code
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  reason, LPVOID lpReserved)
{
	//checks the state of the DLL
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
#elif defined(__linux__)
// Linux-specific code
__attribute__((constructor))
static void DllMainCtor() {
	SpawnClrThread();
}
#endif