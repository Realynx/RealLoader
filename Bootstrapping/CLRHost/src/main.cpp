/*
DLL entry point for running C#
*/

#include "CLR.hpp"

#include <thread>
#include <filesystem>

//finds the desired folder and returns the relative path
static inline PalMM::Util::String FindDotnetDependencyFolderPath_Relative(const char* folderName){

	for (auto& p : std::filesystem::recursive_directory_iterator(".")){
		//if the folder is found, return the path
		if (p.is_directory() && !strcmp(p.path().filename().string().c_str(), folderName))
			return p.path().string();
	}

	printf("RealLoader Error: Failed to find %s at the current directory!\n", folderName);
	return "";
}

//finds the desired folder and returns the absolute path
static inline PalMM::Util::String FindDotnetDependencyFolderPath_Absolute(const char* folderName){

	PalMM::Util::String path = FindDotnetDependencyFolderPath_Relative(folderName);
	if (path.charData.empty()) //if it failed
		return "";

	return std::filesystem::absolute(path.GetCharArray()).string();
}


//runs the CLR thread and DLL
static inline void RUNCLR(){

	PalMM::Util::String appPath;
	PalMM::Util::String configPath;
	//gets the RealLoader Framework folder
	PalMM::Util::String modFrameworkDir = FindDotnetDependencyFolderPath_Absolute("RealLoaderFramework");

	//sets the paths for the CLR runtime
#if defined(_WIN32)
	appPath.SetThickCharData(std::string(modFrameworkDir.charData + "\\RealLoaderFramework.dll").c_str());
	configPath.SetThickCharData(std::string(modFrameworkDir.charData + "\\RealLoaderFramework.runtimeconfig.json").c_str());
#elif defined(__linux__)
	appPath.SetThickCharData(std::string(modFrameworkDir.charData + "/RealLoaderFramework.dll").c_str());
	configPath.SetThickCharData(std::string(modFrameworkDir.charData + "/RealLoaderFramework.runtimeconfig.json").c_str());
#endif

	//init the CLR
	CLR::CLRHost host;
	if (!host.Init(configPath.GetWideCharArray())) {
		std::cout << "Failed To Init Host" << std::endl;
		return;
	}

	std::cout << "Finished Initializing CLR" << std::endl;

	//starts the main Assembly
	host.StartAssembly(appPath.GetWideCharArray());
}



std::atomic<bool> clrInitialized{ false };
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
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  reason, LPVOID lpReserved){

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