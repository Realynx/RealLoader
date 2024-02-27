/*
DLL entry point for running C#
*/

#include "CLR.hpp"

#include <thread>
#include <filesystem>
#include <shellapi.h>
#include <cxxopts.hpp>


//finds the desired folder and returns the relative path
PalMM::Util::String FindDotnetDependencyFolderPath_Relative(const std::string* folderName)
{
	for (auto& p : std::filesystem::recursive_directory_iterator("."))
	{
		//if the folder is found, return the path
		if (p.is_directory() && !strcmp(p.path().filename().string().c_str(), folderName->c_str()))
			return p.path().string();
	}

	printf("Pal World Modding Framework Error: Failed to find %s at the current directory!\n", folderName->c_str());
	return "";
}

//finds the desired folder and returns the absolute path
PalMM::Util::String FindDotnetDependencyFolderPath_Absolute(const std::string* folderName)
{
	printf("Calculating absolute path for %s\n", folderName->c_str());
	std::filesystem::path path(*folderName);
	if (path.is_absolute()) {

	}
	PalMM::Util::String path = FindDotnetDependencyFolderPath_Relative(folderName);
	if (path.charData.empty()) //if it failed
		return "";

	return std::filesystem::absolute(path.GetCharArray()).string();
}


//runs the CLR thread and DLL
void RUNCLR(const std::string frameworkDir)
{
	PalMM::Util::String appPath;
	PalMM::Util::String configPath;
	//gets the Managed Mod Framework folder
	PalMM::Util::String modFrameworkDir = FindDotnetDependencyFolderPath_Absolute(&frameworkDir);

	printf("Found absolute dir for mod framework: %s", modFrameworkDir.GetCharArray());

	//sets the paths for the CLR runtime
#if defined(_WIN32)
	appPath.SetThickCharData(std::string(modFrameworkDir.charData + "\\PalworldManagedModFramework.dll").c_str());
	configPath.SetThickCharData(std::string(modFrameworkDir.charData + "\\PalworldManagedModFramework.runtimeconfig.json").c_str());
#elif defined(__linux__)
	appPath.SetThickCharData(std::string(modFrameworkDir.charData + "/PalworldManagedModFramework.dll").c_str());
	configPath.SetThickCharData(std::string(modFrameworkDir.charData + "/PalworldManagedModFramework.runtimeconfig.json").c_str());
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
void SpawnClrThread(int argc, char** argv) {
	if (clrInitialized.exchange(true)) {
		return;
	}

	cxxopts::Options options("RealLoader CLR Host", "CLR host for enabling C# mods for unreal engine");
	options.add_options()
		("m,modded", "Wether or not to enable mods")
		("f,framework", "The folder to look for the framework in", cxxopts::value<std::string>()->default_value("ManagedModFramework"));

	options.allow_unrecognised_options();

	auto result = options.parse(argc, argv);
	auto modded = result["modded"].as<bool>();

	if (!modded) {
		return;
	}

	//inits console if running on windows.
#if defined(_WIN32)
	AllocConsole();
	freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);
#endif

	auto frameworkDir = &result["framework"].as<std::string>();

	//starts thread and detaches it
	std::thread worker(RUNCLR, *frameworkDir);
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
		SpawnClrThread(__argc, __argv);
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
static void DllMainCtor(int argc, char** argv, char** env) {
	SpawnClrThread(argc, argv);
}
#endif