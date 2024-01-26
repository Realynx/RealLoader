/*
Bootstrapper for hyjacking Unreal Engine 5.1's normal Bootstrapper.
First argument (argv[0]) is the path to the Target Application to start up after we initalize the CLR (Common Language Runtime).
Second argument (argv[1]) is the flag for if the game is being ran modded or vannila.
*/

#include <CLR/CLR.hpp>

#include <iostream>
#include <string>
#include <filesystem>
#include <windows.h>

//entry point
int main(int args, char* argv[])
{
   //if (!args)
   //{
   //    printf("Pal Mod Manager Bootstraper Fatal Error || func: \"main\" || No Args were passed in, we can't get to the real application!\n");
   //    return -1;
   //}
   //
   //// gets the path of the real game, and runs it with the needed arguments
   //std::filesystem::path absolutePath = argv[0];
   //absolutePath = absolutePath.replace_filename("Game-Palworld-Win64-Shipping").string() + ".exe";
   //std::string command = "\"" + absolutePath.string() + "\" " + std::string(argv[1]);
   
    PalModManager::CLR::CLRHost CLRHost;

    // if the game is being ran with mods
    if (true)//!strcmp(argv[2], "-modded"))
    {
        printf("Mods are being initalized Awoooo!\n");

        StaticLibDecl staticLibDecl;
        staticLibDecl.entryPointFuncName = "Hello";
        staticLibDecl.name = "TestLib";
        staticLibDecl.dir = "dfsd";
        printf("AssebleRef Loading || Name: \"%s\", Func Entry Point: \"%s\"\n", staticLibDecl.name.c_str(), staticLibDecl.entryPointFuncName.c_str());

        LoadManagedAssembly_AbsolutePath(L"C:\\Modding\\bin\\Debug-windows-x86_64\\PMMBootstrapper",
           L"TestLib",
           L"Hello");

        printf("Mods have been initalized!\n");
    }

   // system(command.c_str());

    getchar();
    return 0;
}