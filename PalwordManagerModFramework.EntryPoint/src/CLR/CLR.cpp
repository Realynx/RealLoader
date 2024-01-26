//Loads the CLR Host C# Assemblies

#include <CLR/CLR.hpp>

#include <Clr/CoreCLR.hpp>

#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <chrono>
#include <iostream>
#include <thread>
#include <vector>

#include <nethost.h>
#include <coreclr_delegates.h>
#include <hostfxr.h>

#ifdef Window_Build
#include <Windows.h>

#define STR(s) L ## s
#define CH(c) L ## c
#define DIR_SEPARATOR L'\\'

#define string_compare wcscmp

#else

#include <dlfcn.h>
#include <limits.h>

#define STR(s) s
#define CH(c) c
#define DIR_SEPARATOR '/'
#define MAX_PATH PATH_MAX

#define string_compare strcmp

#endif


/********************************************************************************************
 * Function used to load and activate .NET Core
 ********************************************************************************************/

using string_t = std::basic_string<char_t>;

namespace Stuff
{
    // Globals to hold hostfxr exports
    hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr;
    hostfxr_initialize_for_runtime_config_fn init_for_config_fptr;
    hostfxr_get_runtime_delegate_fn get_delegate_fptr;
    hostfxr_run_app_fn run_app_fptr;
    hostfxr_close_fn close_fptr;

    int run_component_example(const string_t& root_path);
    int run_app_example(const string_t& root_path);

#ifdef Window_Build
    void* load_library(const char_t* path)
    {
        HMODULE h = ::LoadLibraryW(path);
        assert(h != nullptr);
        return (void*)h;
    }
    void* get_export(void* h, const char* name)
    {
        void* f = ::GetProcAddress((HMODULE)h, name);
        assert(f != nullptr);
        return f;
    }
#else
    void* load_library(const char_t* path)
    {
        void* h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
        assert(h != nullptr);
        return h;
    }
    void* get_export(void* h, const char* name)
    {
        void* f = dlsym(h, name);
        assert(f != nullptr);
        return f;
    }
#endif

    // <SnippetLoadHostFxr>
    // Using the nethost library, discover the location of hostfxr and get exports
    bool load_hostfxr(const char_t* assembly_path)
    {
        get_hostfxr_parameters params{ sizeof(get_hostfxr_parameters), assembly_path, nullptr };
        // Pre-allocate a large buffer for the path to hostfxr
        char_t buffer[MAX_PATH];
        size_t buffer_size = sizeof(buffer) / sizeof(char_t);
        int rc = get_hostfxr_path(buffer, &buffer_size, &params);
        if (rc != 0)
            return false;

        // Load hostfxr and get desired exports
        void* lib = load_library(buffer);
        init_for_cmd_line_fptr = (hostfxr_initialize_for_dotnet_command_line_fn)get_export(lib, "hostfxr_initialize_for_dotnet_command_line");
        init_for_config_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
        get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
        run_app_fptr = (hostfxr_run_app_fn)get_export(lib, "hostfxr_run_app");
        close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

        return (init_for_config_fptr && get_delegate_fptr && close_fptr);
    }
    // </SnippetLoadHostFxr>

    // <SnippetInitialize>
    // Load and initialize .NET Core and get desired function pointer for scenario
    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* config_path)
    {
        // Load .NET Core
        void* load_assembly_and_get_function_pointer = nullptr;
        hostfxr_handle cxt = nullptr;
        int rc = init_for_config_fptr(config_path, nullptr, &cxt);
        if (rc != 0 || cxt == nullptr)
        {
            std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
            close_fptr(cxt);
            return nullptr;
        }

        // Get the load assembly function pointer
        rc = get_delegate_fptr(
            cxt,
            hdt_load_assembly_and_get_function_pointer,
            &load_assembly_and_get_function_pointer);
        if (rc != 0 || load_assembly_and_get_function_pointer == nullptr)
            std::cerr << "Get delegate failed: " << std::hex << std::showbase << rc << std::endl;

        close_fptr(cxt);
        return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
    }
    // </SnippetInitialize>
}

//loads a managed assembly
bool LoadManagedAssembly_AbsolutePath(const std::wstring _absoluteDir, const std::wstring _fileName, const std::wstring _entryFuncName)
{
    // STEP 1: Load HostFxr and get exported hosting functions
     //
    if (!Stuff::load_hostfxr(nullptr))
    {
        assert(false && "Failure: load_hostfxr()");
        return EXIT_FAILURE;
    }

    //
    // STEP 2: Initialize and start the .NET Core runtime
    //
    const std::wstring config_path = _absoluteDir + DIR_SEPARATOR + _fileName + STR(".runtimeconfig.json");
    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
    load_assembly_and_get_function_pointer = Stuff::get_dotnet_load_assembly(config_path.c_str());
    assert(load_assembly_and_get_function_pointer != nullptr && "Failure: get_dotnet_load_assembly()");

    //
    // STEP 3: Load managed assembly and get function pointer to a managed method
    //
    const std::wstring dotnetlib_path = _absoluteDir /*std::wstring(STR("C:\\Modding\\bin\\Debug-windows-x86_64\\PMMBootstrapper"))*/ + DIR_SEPARATOR +
        _fileName + STR(".dll");
    const std::wstring dotnet_type = STR("TestLib.Lib, TestLib");
    const std::wstring dotnet_type_method = STR("Hello");
    // <SnippetLoadAndGet>
    // Function pointer to managed delegate
    component_entry_point_fn hello = nullptr;
    int rc = load_assembly_and_get_function_pointer(
        dotnetlib_path.c_str(),
        dotnet_type.c_str(),
        dotnet_type_method.c_str(),
        nullptr /*delegate_type_name*/,
        nullptr,
        (void**)&hello);
    // </SnippetLoadAndGet>
    assert(rc == 0 && hello != nullptr && "Failure: load_assembly_and_get_function_pointer()");

    //
    // STEP 4: Run managed code
    //
    struct lib_args
    {
        const char_t* message;
        int number;
    };
    for (int i = 0; i < 3; ++i)
    {
        // <SnippetCallManaged>
        lib_args args
        {
            STR("from host!"),
            i
        };

        hello(&args, sizeof(args));
        // </SnippetCallManaged>
    }

    // Function pointer to managed delegate with non-default signature
    typedef void (CORECLR_DELEGATE_CALLTYPE* custom_entry_point_fn)(lib_args args);
    custom_entry_point_fn custom = nullptr;
    lib_args args
    {
        STR("from host!"),
        -1
    };

    // UnmanagedCallersOnly
    rc = load_assembly_and_get_function_pointer(
        (wchar_t*)dotnetlib_path.c_str(),
        (wchar_t*)dotnet_type.c_str(),
        STR("CustomEntryPointUnmanagedCallersOnly") /*method_name*/,
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr,
        (void**)&custom);
    assert(rc == 0 && custom != nullptr && "Failure: load_assembly_and_get_function_pointer()");
    custom(args);

    // Custom delegate type
    rc = load_assembly_and_get_function_pointer(
        dotnetlib_path.c_str(),
        dotnet_type.c_str(),
        STR("CustomEntryPoint") /*method_name*/,
        std::wstring(_fileName + std::wstring(L".Lib+CustomEntryPointDelegate, ") + _fileName).c_str(),
        nullptr,
        (void**)&custom);
    assert(rc == 0 && custom != nullptr && "Failure: load_assembly_and_get_function_pointer()");
    custom(args);

    return EXIT_SUCCESS;
}
//
////load a managed C# Assembly by a relative path
//bool LoadManagedAssembly_RelativePath(const char* relativePath, const StaticLibDecl& staticLibDecl)
//{
//    // Get the current executable's directory
//    // This sample assumes the managed assembly to load and its runtime configuration file are next to the host
//    char_t host_path[MAX_PATH];
//#if Window_Build
//    auto size = ::GetFullPathNameW((const WCHAR*)relativePath, sizeof(host_path) / sizeof(char_t), host_path, nullptr);
//    assert(size != 0);
//#else
//    auto resolved = realpath(argv[0], host_path);
//    assert(resolved != nullptr);
//#endif
//
//    std::wstring(L root_path = host_path;
//    auto pos = root_path.find_last_of(DIR_SEPARATOR);
//    assert(pos != std::wstring(L::npos);
//    root_path = root_path.substr(0, pos + 1);
//
//    return Stuff::run_component_example(root_path);
//
//    //if (argc > 1 && string_compare(argv[1], STR("app")) == 0)
//    //{
//    //    return run_app_example(root_path);
//    //}
//    //else
//    //{
//    //    return run_component_example(root_path);
//    //}
//
//	return true;
//}

namespace Stuff
{
    //int run_component_example(const std::wstring(L& root_path)
    //{
    //    //
    //     // STEP 1: Load HostFxr and get exported hosting functions
    //     //
    //    if (!load_hostfxr(nullptr))
    //    {
    //        assert(false && "Failure: load_hostfxr()");
    //        return EXIT_FAILURE;
    //    }

    //    //
    //    // STEP 2: Initialize and start the .NET Core runtime
    //    //
    //    const std::wstring(L config_path = root_path + STR("TestLib.runtimeconfig.json");
    //    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
    //    load_assembly_and_get_function_pointer = get_dotnet_load_assembly(config_path.c_str());
    //    assert(load_assembly_and_get_function_pointer != nullptr && "Failure: get_dotnet_load_assembly()");

    //    //
    //    // STEP 3: Load managed assembly and get function pointer to a managed method
    //    //
    //    const std::wstring(L dotnetlib_path = root_path + STR("TestLib.dll");
    //    const char_t* dotnet_type = STR("TestLib.Lib, TestLib");
    //    const char_t* dotnet_type_method = STR("Hello");
    //    // <SnippetLoadAndGet>
    //    // Function pointer to managed delegate
    //    component_entry_point_fn hello = nullptr;
    //    int rc = load_assembly_and_get_function_pointer(
    //        dotnetlib_path.c_str(),
    //        dotnet_type,
    //        dotnet_type_method,
    //        nullptr /*delegate_type_name*/,
    //        nullptr,
    //        (void**)&hello);
    //    // </SnippetLoadAndGet>
    //    assert(rc == 0 && hello != nullptr && "Failure: load_assembly_and_get_function_pointer()");

    //    //
    //    // STEP 4: Run managed code
    //    //
    //    struct lib_args
    //    {
    //        const char_t* message;
    //        int number;
    //    };
    //    for (int i = 0; i < 3; ++i)
    //    {
    //        // <SnippetCallManaged>
    //        lib_args args
    //        {
    //            STR("from host!"),
    //            i
    //        };

    //        hello(&args, sizeof(args));
    //        // </SnippetCallManaged>
    //    }

    //    // Function pointer to managed delegate with non-default signature
    //    typedef void (CORECLR_DELEGATE_CALLTYPE* custom_entry_point_fn)(lib_args args);
    //    custom_entry_point_fn custom = nullptr;
    //    lib_args args
    //    {
    //        STR("from host!"),
    //        -1
    //    };

    //    // UnmanagedCallersOnly
    //    rc = load_assembly_and_get_function_pointer(
    //        dotnetlib_path.c_str(),
    //        dotnet_type,
    //        STR("CustomEntryPointUnmanagedCallersOnly") /*method_name*/,
    //        UNMANAGEDCALLERSONLY_METHOD,
    //        nullptr,
    //        (void**)&custom);
    //    assert(rc == 0 && custom != nullptr && "Failure: load_assembly_and_get_function_pointer()");
    //    custom(args);

    //    // Custom delegate type
    //    rc = load_assembly_and_get_function_pointer(
    //        dotnetlib_path.c_str(),
    //        dotnet_type,
    //        STR("CustomEntryPoint") /*method_name*/,
    //        STR("TestLib.Lib+CustomEntryPointDelegate, TestLib") /*delegate_type_name*/,
    //        nullptr,
    //        (void**)&custom);
    //    assert(rc == 0 && custom != nullptr && "Failure: load_assembly_and_get_function_pointer()");
    //    custom(args);

    //    return EXIT_SUCCESS;
    //}

    int run_app_example(const string_t& root_path)
    {
        const string_t app_path = root_path + STR("App.dll");

        if (!load_hostfxr(app_path.c_str()))
        {
            assert(false && "Failure: load_hostfxr()");
            return EXIT_FAILURE;
        }

        // Load .NET Core
        hostfxr_handle cxt = nullptr;
        std::vector<const char_t*> args{ app_path.c_str(), STR("app_arg_1"), STR("app_arg_2") };
        int rc = init_for_cmd_line_fptr(args.size(), args.data(), nullptr, &cxt);
        if (rc != 0 || cxt == nullptr)
        {
            std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
            close_fptr(cxt);
            return EXIT_FAILURE;
        }

        // Get the function pointer to get function pointers
        get_function_pointer_fn get_function_pointer;
        rc = get_delegate_fptr(
            cxt,
            hdt_get_function_pointer,
            (void**)&get_function_pointer);
        if (rc != 0 || get_function_pointer == nullptr)
            std::cerr << "Get delegate failed: " << std::hex << std::showbase << rc << std::endl;

        // Function pointer to App.IsWaiting
        typedef unsigned char (CORECLR_DELEGATE_CALLTYPE* is_waiting_fn)();
        is_waiting_fn is_waiting;
        rc = get_function_pointer(
            STR("App, App"),
            STR("IsWaiting"),
            UNMANAGEDCALLERSONLY_METHOD,
            nullptr, nullptr, (void**)&is_waiting);
        assert(rc == 0 && is_waiting != nullptr && "Failure: get_function_pointer()");

        // Function pointer to App.Hello
        typedef void (CORECLR_DELEGATE_CALLTYPE* hello_fn)(const char*);
        hello_fn hello;
        rc = get_function_pointer(
            STR("App, App"),
            STR("Hello"),
            UNMANAGEDCALLERSONLY_METHOD,
            nullptr, nullptr, (void**)&hello);
        assert(rc == 0 && hello != nullptr && "Failure: get_function_pointer()");

        // Invoke the functions in a different thread from the main app
        std::thread t([&]
            {
                while (is_waiting() != 1)
                    std::this_thread::sleep_for(std::chrono::milliseconds(100));

                for (int i = 0; i < 3; ++i)
                    hello("from host!");
            });

        // Run the app
        run_app_fptr(cxt);
        t.join();

        close_fptr(cxt);
        return EXIT_SUCCESS;
    }
}


//directory where the CLR runtime 
//#define PAL_MOD_MANAGER_CORE_CLR_DIR "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App"

//the version to check for
//#define PAL_MOD_MANAGER_CORE_CLR_VER_DIR "8.0.1"

////makes Trust Platform Assembled List
//static inline void BuildTpaList(char* directory, std::string& tpaList)
//{
//	std::string dir = std::string(directory) + "\\*.dll";
//
//	WIN32_FIND_DATAA findData;
//	HANDLE fileHandle = FindFirstFileA(dir.c_str(), &findData);
//
//	//generate a semi-colon seperate string list
//	if (fileHandle != INVALID_HANDLE_VALUE)
//	{
//		//while (FindNextFileA(fileHandle, &findData))
//		//{
//		//	tpaList += directory + "\\" + findData.cFileName + ";";
//		//}
//
//		do
//		{
//			tpaList.append(directory);
//			tpaList.append("\\");
//			tpaList.append(findData.cFileName);
//			tpaList.append(";");
//		} while (FindNextFileA(fileHandle, &findData));
//
//		FindClose(fileHandle);
//	}
//}
//
////makes Trust Platform Assembled List
//static inline void BuildTpaList(std::string& directory, std::string& tpaList)
//{
//	directory += "\\*.dll";
//
//	WIN32_FIND_DATAA findData;
//	HANDLE fileHandle = FindFirstFileA(directory.c_str(), &findData);
//
//	//generate a semi-colon seperate string list
//	if (fileHandle != INVALID_HANDLE_VALUE)
//	{
//		while (FindNextFileA(fileHandle, &findData))
//		{
//			tpaList += directory + "\\" + findData.cFileName + ";";
//		}
//
//		/*do
//		{
//			tpaList.append(directory);
//			tpaList.append("\\");
//			tpaList.append(findData.cFileName);
//			tpaList.append(";");
//		} while (FindNextFileA(fileHandle, &findData));*/
//
//		FindClose(fileHandle);
//	}
//}
//
////creates the pointer to the Mod DLL
//PalModManager::CLR::managed_direct_method_ptr PalModManager::CLR::CLRHost::CreateManagedDelegate(const char* managedAssemblyName, const char* managedClassName, const char* managedsubroutineName)
//{
//	auto createManagedDelegate = (coreclr_create_delegate_ptr)GetProcAddress(_coreClr, "coreclr_create_delegate");
//
//	if (createManagedDelegate == NULL) {
//		printf("'coreclr_create_delegate' could not be found within coreclr.dll!\n");
//		return NULL;
//	}
//
//	// The CLR finds the assembly from the TPA list we created it with.
//	managed_direct_method_ptr managedSubroutine;
//	int hResponse = createManagedDelegate(_hostHandle, _domainId,
//		managedAssemblyName,
//
//		managedClassName,
//		managedsubroutineName,
//		(void**)&managedSubroutine);
//
//	if (hResponse >= 0) {
//		printf("Created Managed Delegate to: '%s::%s'\n", managedClassName, managedsubroutineName);
//		return managedSubroutine;
//	}
//
//	printf("'coreclr_create_delegate' failed to create the delegate with error code: %x\n", hResponse);
//	return NULL;
//}
//
////load a managed C# Assemvly
//bool PalModManager::CLR::CLRHost::LoadManagedAssembly(const char* filePath, const char* appDomainId, const char* managedAssemblyName, const char* entryClassName, const char* entrySubroutine)
//{
//	char runtimeDirectory[MAX_PATH];
//	//std::string runtimeDirectory; runtimeDirectory.resize(MAX_PATH);
//
//	GetFullPathNameA(filePath, MAX_PATH, runtimeDirectory, NULL);
//
//	std::string coreCLRDir = std::string(std::string(PAL_MOD_MANAGER_CORE_CLR_DIR) + "\\" + std::string(PAL_MOD_MANAGER_CORE_CLR_VER_DIR));
//	std::string coreClrAssembly = std::string(std::string(PAL_MOD_MANAGER_CORE_CLR_DIR) + "\\" + std::string(PAL_MOD_MANAGER_CORE_CLR_VER_DIR) + std::string("\\coreclr.dll"));
//
//	printf("Full Assembly Name: %s\n", managedAssemblyName);
//	printf("Loading Directory: %s\n", runtimeDirectory);
//	printf("Loading Runtime: %s\n", coreClrAssembly.c_str());
//
//	_coreClr = LoadLibraryExA(coreClrAssembly.c_str(), NULL, 0);
//
//	if (_coreClr == NULL) {
//		printf("There was an error loading coreclr.dll!\n");
//		return false;
//	}
//
//	printf("Found and loaded coreclr.dll\n");
//
//	auto initializeCoreClr = (coreclr_initialize_ptr)GetProcAddress(_coreClr, "coreclr_initialize");
//
//	if (initializeCoreClr == NULL) {
//		printf("Could not find 'coreclr_initialize' subroutine in coreclr.dll!\n");
//		return false;
//	}
//
//	printf("Building Trusted Platform Assemblies list...\n");
//
//	std::string tpaList = "", dir = coreCLRDir;
//	BuildTpaList(dir, tpaList);
//	BuildTpaList(runtimeDirectory, tpaList);
//
//	printf("Trusted Platform Assemblies list complete.\n");
//
//	const char* propertyKeys[] = { "TRUSTED_PLATFORM_ASSEMBLIES", "APP_CONTEXT_BASE_DIRECTORY" };
//	const char* propertyValues[] = { tpaList.c_str(), runtimeDirectory};
//
//	int hResponse = initializeCoreClr(
//		coreCLRDir.c_str(),
//		appDomainId,
//		sizeof(propertyKeys) / sizeof(char*),
//		propertyKeys,
//		propertyValues,
//		&_hostHandle,
//		&_domainId);
//
//	if (hResponse >= 0) {
//		printf("CoreClr has started!\n");
//		_managedDirectMethod = CreateManagedDelegate(managedAssemblyName, entryClassName, entrySubroutine);
//		if (!_managedDirectMethod)
//		{
//			printf("Pal Mod Manager || CLR || Func \"LoadManagedAssembly\" || Failed to create a Managed C# Delegate!\n");
//			return false;
//		}
//
//		_managedDirectMethod();
//		return true;
//	}
//
//	printf("CoreClr 'initializeCoreClr' has failed with response code: %x\n", hResponse);
//	return true;
//}