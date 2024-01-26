#pragma once

//Loads the CLR Host C# Assemblies

#include <Windows.h>

#include <string>

#include <nethost.h>
#include <coreclr_delegates.h>
#include <hostfxr.h>

#include <queue>

#define PMM_WSTR(s) L ## s

namespace PalModManager::CLR
{
	//defines a static lib loaded in
	struct LoadedDLLs
	{
		component_entry_point_fn funcEntryPoint = nullptr; //the function pointer to the entry point func for the application

		std::wstring appName = PMM_WSTR(""); //the name of the application
		std::wstring dllDir = PMM_WSTR(""); //the directory of the directory
		std::wstring entryFuncName = PMM_WSTR(""); //the name of the entry point func of the application
	};

	//pointer to the managed method entry point
	//typedef char* (*managed_direct_method_ptr)();

	//defines a loaded asse

	//defines the CLR Host
	struct CLRHost
	{
		std::queue<LoadedDLLs> loadedDLLs; //the loaded DLLs

		//inits the Host

		//shutsdown the Host

		//loads a DLL
		bool LoadManagedAssembly_AbsolutePath(const std::wstring _absoluteDir,
			const std::wstring _fileName,
			const std::wstring _namespace = L"PalworldManagedModFramework", const std::wstring _typeName = L"Program", const std::wstring _entryFuncName = L"ManagedEntryPoint");

		//unloads a DLL

		//gets a DLL
	};
}

