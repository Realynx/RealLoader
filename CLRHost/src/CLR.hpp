#pragma once

//defines a CLR Host for initalizing C# code and executing it

#include <assert.h>
#include <iostream>

#include <nethost.h>
#include <coreclr_delegates.h>
#include <hostfxr.h>

//#ifdef Window_Build
#include <Windows.h>

#define STR(s) L ## s
#define CH(c) L ## c
#define DIR_SEPARATOR L'\\'

#define string_compare wcscmp

//#else
//#include <dlfcn.h>
//#include <limits.h>
//
//#define STR(s) s
//#define CH(c) c
//#define DIR_SEPARATOR '/'
//#define MAX_PATH PATH_MAX
//
//#define string_compare strcmp

//#endif

 //------------------------------------Function used to load and activate .NET Core-------------------------//

namespace CLR::Util
{

	//#ifdef Window_Build
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
	//#else
	//    void* load_library(const char_t* path)
	//    {
	//        void* h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
	//        assert(h != nullptr);
	//        return h;
	//    }
	//    void* get_export(void* h, const char* name)
	//    {
	//        void* f = dlsym(h, name);
	//        assert(f != nullptr);
	//        return f;
	//    }
	//#endif
}

//----------------------------------MAIN CLR-----------------------//

namespace CLR
{
	//defines a CLR Host
	struct CLRHost
	{
		// Globals to hold hostfxr exports
		/*hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr;*/
		hostfxr_handle cxt = nullptr;

		hostfxr_initialize_for_runtime_config_fn init_for_config_fptr;
		hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate;

		hostfxr_run_app_fn run_app_fptr;
		hostfxr_close_fn close_fptr;

		//inits the CLR Host
		inline bool Init(const char_t* config_Path)
		{
			get_hostfxr_parameters params{ sizeof(get_hostfxr_parameters), nullptr, nullptr };
			// Pre-allocate a large buffer for the path to hostfxr
			char_t buffer[MAX_PATH];
			size_t buffer_size = sizeof(buffer) / sizeof(char_t);

			int rc = get_hostfxr_path(buffer, &buffer_size, &params);
			if (rc != 0)
				return false;

			// Load hostfxr and get desired exports
			void* lib = Util::load_library(buffer);

			init_for_config_fptr = (hostfxr_initialize_for_runtime_config_fn)Util::get_export(lib, "hostfxr_initialize_for_runtime_config");
			hostfxr_get_runtime_delegate = (hostfxr_get_runtime_delegate_fn)Util::get_export(lib, "hostfxr_get_runtime_delegate");

			run_app_fptr = (hostfxr_run_app_fn)Util::get_export(lib, "hostfxr_run_app");
			close_fptr = (hostfxr_close_fn)Util::get_export(lib, "hostfxr_close");

			auto runtimeConfig = std::string(CStringA(config_Path).GetString());
			std::cout << "Using config: " << runtimeConfig << std::endl;
			rc = init_for_config_fptr(config_Path, nullptr, &cxt);
			if (rc != 0 || cxt == nullptr)
			{
				std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
				return false;
			}


			return (init_for_config_fptr && hostfxr_get_runtime_delegate);
		}

		typedef int (*GetManagedFunctionPointer)(const char_t*, const char_t*, const char_t*, void*, void*, void**);
		typedef int (*LoadAssembly)(const char_t*, void*, void*);
		typedef void(*ManagedEntrypoint)();

		void StartAssembly(const char_t* assemblyPath) {
			if (!hostfxr_get_runtime_delegate) {
				std::cout << "hostfxr_get_runtime_delegate was null" << std::endl;
				return;
			}

			std::cout << "Top Start Assembly" << std::endl;

			LoadAssembly loadAssembly = nullptr;
			hostfxr_get_runtime_delegate(cxt, hdt_load_assembly, (void**)&loadAssembly);
			if (!loadAssembly) {
				std::cout << "Load Assembly not found" << std::endl;

				auto rc = GetLastError();
				std::cout << rc << std::endl;
				return;
			}
			std::cout << "Load Assembly found" << std::endl;


			GetManagedFunctionPointer getManagedFunctionPtr = nullptr;
			hostfxr_get_runtime_delegate(cxt, hdt_get_function_pointer, (void**)&getManagedFunctionPtr);
			if (!getManagedFunctionPtr) {
				std::cout << "managedEntryPoint not found" << std::endl;

				auto rc = GetLastError();
				std::cout << rc << std::endl;
				return;
			}
			std::cout << "managedEntryPoint found" << std::endl;


			std::cout << "Calling loadAssembly" << std::endl;
			loadAssembly(assemblyPath, NULL, NULL);
			std::cout << "Called loadAssembly" << std::endl;

			ManagedEntrypoint managedEntrypoint = nullptr;
			std::cout << "Calling managedEntryPoint" << std::endl;
			getManagedFunctionPtr(STR("PalworldManagedModFramework.Program, PalworldManagedModFramework"), STR("EntryPoint"), STR("PalworldManagedModFramework.Program+VoidDelegateSignature, PalworldManagedModFramework"), NULL, NULL, (void**)&managedEntrypoint);
			std::cout << "Called managedEntryPoint" << std::endl;

			if (!managedEntrypoint) {
				std::cout << "UnmanagedEntrypoint EntryPoint was NULL :'(" << std::endl;

				auto rc = GetLastError();
				std::cout << rc << std::endl;
				return;
			}
			std::cout << "UnmanagedEntrypoint EntryPoint was FOUND!!!" << std::endl;

			std::cout << "Calling C# Code: 0x" << std::hex << (int)managedEntrypoint << std::endl;
			managedEntrypoint();
			std::cout << "Called" << std::endl;

			close_fptr(cxt);
		}
	};
}
