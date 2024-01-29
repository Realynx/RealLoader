/*
DLL entry point for running C#
*/

#include "CLR.hpp"

#include <thread>

//runs the CLR thread and DLL
void RUNCLR()
{
	CLR::CLRHost host;;
	std::basic_string<char_t> app_path = L"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld\\Pal\\Binaries\\Win64\\ManagedModFramework\\PalworldManagedModFramework.dll";

	//init the CLR
	auto configPath = STR("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld\\Pal\\Binaries\\Win64\\ManagedModFramework\\PalworldManagedModFramework.runtimeconfig.json");
	if (!host.Init(configPath))
	{
		std::cout << "Failed To Init ^2\n";
		return;
	}
	std::cout << "Finished Initalizing CLR\n";

	//starts the main Assembly
	host.StartAssembly(app_path.c_str());
}

void SpawnClrThread() {

	//inits console
	AllocConsole();
	freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);

	//starts thread and detachtes it
	std::thread worker(RUNCLR);
	worker.detach();
}

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