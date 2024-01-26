#pragma once

//Loads the CLR Host C# Assemblies

#define LEAN_AND_MEAN
#include <Windows.h>

#include <string>

namespace PalModManager::CLR
{
    //pointer to the managed method entry point
    typedef char* (*managed_direct_method_ptr)();

    //defines the CLR Host
    class CLRHost
    {
    private:
        HMODULE _coreClr;
        void* _hostHandle;
        unsigned int _domainId;
        managed_direct_method_ptr _managedDirectMethod;

        //creates the pointer to the Mod DLL
        managed_direct_method_ptr CreateManagedDelegate(const char* managedAssemblyName, const char* managedClassName, const char* managedsubroutineName);

    public:

        //load a managed C# Assemvly
        bool LoadManagedAssembly(const char* filePath, const char* appDomainId, const char* managedAssemblyName, const char* entryClassName, const char* entrySubroutine);
    };
}