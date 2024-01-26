#include "CLR.hpp"

#include <iostream>
#include <string>
#include <filesystem>
#include <windows.h>

//entry point
int main(int args, char* argv[])
{
	if (!args)
	{
		printf("Pal Mod Manager Bootstraper Fatal Error || func: \"main\" || No Args were passed in, we can't get to the real application!\n");
		return -1;
	}

	// gets the path of the real game, and runs it with the needed arguments
	std::filesystem::path absolutePath = argv[0];
	absolutePath = absolutePath.replace_filename("Game-Palworld-Win64-Shipping").string() + ".exe";
	std::string command = "\"" + absolutePath.string() + "\" " + std::string(argv[1]);

	PalModManager::CLR::CLRHost CLRHost;

	// if the game is being ran with mods
	if (!strcmp(argv[2], "-modded"))
	{
		printf("Mods are being initalized Awoooo!\n");

		CLRHost.LoadManagedAssembly("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld\\Pal\\Binaries\\Win64", "AppID",
			//"ConsoleApp1", "Program", "Main");
			"PalworldManagedModFramework", //Assembly Name
			"Program", //Program Name
			"ManagedEntryPoint"); //Func Entry Point

		printf("Mods have been initalized!\n");
	}

	system(command.c_str());

	getchar();
	return 0;
}