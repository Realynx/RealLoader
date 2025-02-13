/*
DLL entry point for running C#
*/

#include "CLR.hpp"

#include <thread>
#include <filesystem>

//gets the dotnet dependency folder path
static inline RealLoader::Util::RealString FindDotnetDependencyFolderPath(const char_t* folderName)
{
	for (auto& p : std::filesystem::recursive_directory_iterator("."))
	{
		if (p.is_directory() && RealLoader::Util::IsSameString((char_t*)p.path().filename().string().c_str(), folderName))
			return p.path().string();
	}

	RealLoader::Util::RealString msg = STR("Failed to find Dotnet folder at \"");
	msg += RealLoader::Util::RealString(folderName) + STR("\"");
	RealLoader::Util::LogError(STR("Failed To Find Dotnet Folder"), msg);
	return RealLoader::Util::RealString();
}

//runs the CLR thread and DLL
static inline void RUNCLR() {

	//gets the RealLoader Framework folder
	RealLoader::Util::RealString modFrameworkDir = FindDotnetDependencyFolderPath(STR("RealLoaderFramework"));

	//sets the paths for the CLR runtime
	RealLoader::Util::RealString appPath = modFrameworkDir.data + DIR + STR("RealLoaderFramework.dll");
	RealLoader::Util::RealString configPath = modFrameworkDir.data + + DIR + STR("RealLoaderFramework.runtimeconfig.json");

	//init the CLR
	CLR::CLRHost host;
	if (!host.Init(configPath.data.c_str())) {
		RealLoader::Util::LogFatalError(STR("CLR Init"), STR("Failed to initalize CLR Host!!!"));
		return;
	}

	RealLoader::Util::LogMessage(STR("Finished initalizing CLR!"));

	//starts the main Assembly
	host.StartAssembly(appPath.data.c_str());
}


std::atomic<bool> clrInitialized{false};

static inline void SpawnClrThread() {
	if (clrInitialized.exchange(true)) {
		return;
	}

	//inits console if running on windows.
#if defined(_WIN32)
	AllocConsole();
	freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);
#endif

	//starts thread and detaches it
	std::thread worker(RUNCLR);
	worker.detach();
}


#if defined(_WIN32)
// Windows-specific code
BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID lpReserved) {
	//checks the state of the DLL
	switch (reason) {
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