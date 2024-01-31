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
	PalMM::Util::String appPath; appPath.SetCharData(STR("Pal\\Binaries\\Win64\\ManagedModFramework\\PalworldManagedModFramework.dll"));
	PalMM::Util::String configPath; configPath.SetCharData(STR("Pal\\Binaries\\Win64\\ManagedModFramework\\PalworldManagedModFramework.runtimeconfig.json"));
#elif defined(__linux__)
	PalMM::Util::String appPath; appPath.SetCharData(STR("Pal/Binaries/Linux/ManagedModFramework/PalworldManagedModFramework.dll"));
	PalMM::Util::String configPath = STR("Pal/Binaries/Linux/ManagedModFramework/PalworldManagedModFramework.runtimeconfig.json");
#endif

	PalMM::Util::String fullAppPath; fullAppPath.SetThickCharData(std::filesystem::absolute(appPath.GetCharArray()).string().c_str());
	PalMM::Util::String fullConfigPath; fullConfigPath.SetThickCharData(std::filesystem::absolute(configPath.GetCharArray()).string().c_str());

	//init the CLR
	CLR::CLRHost host;
	if (!host.Init(fullConfigPath.GetWideCharArray())) {
		std::cout << "Failed To Init Host" << std::endl;
		return;
	}

	std::cout << "Finished Initializing CLR" << std::endl;

	//starts the main Assembly
	host.StartAssembly(fullAppPath.GetWideCharArray());
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