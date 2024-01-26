#pragma once

//Loads the CLR Host C# Assemblies

#include <Windows.h>

#include <string>

#include <nethost.h>

namespace PalModManager::CLR
{
	//pointer to the managed method entry point
	//typedef char* (*managed_direct_method_ptr)();

	//defines a loaded asse

	//defines the CLR Host
	struct CLRHost
	{
		//load a managed C# Assembly by a relative path
		//bool LoadManagedAssembly_RelativePath(const char* filePath, const char* appDomainId, const char* managedAssemblyName, const char* entryClassName, const char* entrySubroutine);
	};
}

//loads a managed assembly
bool LoadManagedAssembly_AbsolutePath(const std::wstring _absoluteDir, const std::wstring _fileName, const std::wstring _entryFuncName);

//defines a static lib
struct StaticLibDecl
{
	std::string name = "";
	std::string entryPointFuncName;
	std::string dir = "";
};

//load a managed C# Assembly by a relative path
//bool LoadManagedAssembly_RelativePath(const char* relativePath, const StaticLibDecl& staticLibDecl);