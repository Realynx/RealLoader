#pragma once

//defines a CLR Host for initalizing C# code and executing it

#if defined(_WIN32)

//include a stripped windows.h
#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#endif

//assert and output
#include <assert.h>
#include <iostream>

//Host CLR
#include <nethost.h>
#include <coreclr_delegates.h>
#include <hostfxr.h>

//custom string for thick and C char arrays
#include <String.hpp>

//since GetLastError doesn't exit on Linux we define it
#if defined(__linux__)
#include <stdlib.h>
#include <stdio.h>
#include <dlfcn.h>

int GetLastError() { return 0; }
#endif

//-------------------------UTILS FOR LOADING LIBRARIES AT RUNTIME FROM DLLS-------------------------------//

namespace CLR::Util {

#if defined(_WIN32)

	//loads a library
	static inline void* LoadDLLibrary(const char_t* path){

		HMODULE h = ::LoadLibraryW(path);
		assert(h != nullptr);
		return (void*)h;
	}

	//gets a exported DLL function
	static inline void* GetDLLExportedFunction(void* h, const char* name) {

		void* f = ::GetProcAddress((HMODULE)h, name);
		assert(f != nullptr);
		return f;
	}

#elif defined(__linux__)
	static inline void* LoadDLLibrary(const char_t* path){

		void* h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
		assert(h != nullptr);
		return h;
	}
	static inline void* GetDLLExportedFunction(void* h, const char* name){

		void* f = dlsym(h, name);
		assert(f != nullptr);
		return f;
	}
#endif
}

//----------------------------------MAIN CLR-----------------------//

namespace CLR {

	//defines a CLR Host
	struct CLRHost {

		hostfxr_handle cxt = nullptr; //the context for the CLR Host

		//the function pointer handles
		hostfxr_initialize_for_runtime_config_fn hostfxr_funcPtr_initConfig; //initializes the runtime and it's config data

		hostfxr_set_runtime_property_value_fn hostfxr_funcPtr_SetRuntimeProperty; //sets the runtime property
		hostfxr_get_runtime_delegate_fn hostfxr_funcPtr_GetRuntimeDelegate; //gets a function from managed C# land

		hostfxr_close_fn hostfxr_funcPtr_Close; //closes the CLR, we don't use it as far as this comment knows (dated 01-29-24)

	public:

		//inits the CLR Host
		inline bool Init(const char_t* config_Path){

			//gets the hostfxr path automatically
			get_hostfxr_parameters params{ sizeof(get_hostfxr_parameters), nullptr, nullptr };
			char_t buffer[999];
			size_t buffer_size = sizeof(buffer) / sizeof(char_t);
			if (get_hostfxr_path(buffer, &buffer_size, &params) != 0){

				std::cout << "Failed to find HostFXR on your system! Please make sure you have the Dot Net SDK or Runtime installed. Thank you.\n";
				return false;
			}

			//loads hostfxr
			void* hostfxr_lib = Util::LoadDLLibrary(buffer);

			//gets the desired functions from the Hostfxr DLL
			hostfxr_funcPtr_SetRuntimeProperty = (hostfxr_set_runtime_property_value_fn)Util::GetDLLExportedFunction(hostfxr_lib, "hostfxr_set_runtime_property_value");

			hostfxr_funcPtr_initConfig = (hostfxr_initialize_for_runtime_config_fn)Util::GetDLLExportedFunction(hostfxr_lib, "hostfxr_initialize_for_runtime_config");
			hostfxr_funcPtr_GetRuntimeDelegate = (hostfxr_get_runtime_delegate_fn)Util::GetDLLExportedFunction(hostfxr_lib, "hostfxr_get_runtime_delegate");

			hostfxr_funcPtr_Close = (hostfxr_close_fn)Util::GetDLLExportedFunction(hostfxr_lib, "hostfxr_close");

			//initializes the config
			const auto runtimeConfig = PalMM::Util::ConvertThickStringToCString(config_Path);
			std::cout << "Using config: " << runtimeConfig << std::endl;
			int rc = hostfxr_funcPtr_initConfig(config_Path, nullptr, &cxt);
			if (cxt == nullptr || rc != 0){

				std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
				return false;
			}

			//sets the app base config
			SetAppContextBase(config_Path);

			return (hostfxr_funcPtr_SetRuntimeProperty && hostfxr_funcPtr_initConfig && hostfxr_funcPtr_GetRuntimeDelegate && hostfxr_funcPtr_Close);
		}

#if defined(_WIN32)
		//sets a property in the CLR
		inline void SetCLRProperty(const wchar_t* propertyStr, const wchar_t* newValue, bool ignoreLogging = false) {

#else
		//sets a property in the CLR
		inline void SetCLRProperty(const char* propertyStr, const char* newValue, bool ignoreLogging = false) {
#endif 
		
			if (!hostfxr_funcPtr_SetRuntimeProperty) {
				if (!ignoreLogging) {
					std::cout << "RealLoader Error: CLR || Func: \"SetCLRProperty\" || Failed to set \"" << propertyStr << "\" to Value \"" << newValue << "\" due to hostfxr_funcPtr_SetRuntimeProperty being NULL. Please check that HostFXR hasn't failed at being found on your system. And any other previous CLR initialize states are running as expected.\n";
				}

				return;
			}

			std::cout << "OwO Property Setting: \"" << PalMM::Util::ConvertThickStringToCString(propertyStr) << "\" || \"" << PalMM::Util::ConvertThickStringToCString(newValue) << "\"\n";
			hostfxr_funcPtr_SetRuntimeProperty(cxt, propertyStr, newValue);
		}

		//sets the base app context || takes in a file to the config path, we strip out the file name and just use the directory
		inline void SetAppContextBase(const char_t* configPath) {
			//calculate and set up the base directory for the app context
			std::string baseAppContextDir = PalMM::Util::ConvertThickStringToCString(configPath);

#if defined(_WIN32)
			auto pos = baseAppContextDir.find_last_of("\\");
#else
			auto pos = baseAppContextDir.find_last_of("/");
#endif

			baseAppContextDir = baseAppContextDir.substr(0, pos + 1);

			SetCLRProperty(STR("APP_CONTEXT_BASE_DIRECTORY"), PalMM::Util::ConvertCStringToThickString(baseAppContextDir).c_str());
		}

		//function pointer types for handling DLLs and their managed code
		typedef int (*LoadAssembly)(const char_t*, void*, void*); //loads a DLL
		typedef int (*GetManagedFunctionPointer)(const char_t*, const char_t*, const char_t*, void*, void*, void**); //gets a function from C# land
		typedef void(*ManagedEntrypoint)(); //calls a C# function

		//loads an assembly and calls it's entry point
		inline void StartAssembly(const char_t* assemblyPath) {

			//can't do anything if the function from Hostfxr for loading C# functions isn't found
			if (!hostfxr_funcPtr_GetRuntimeDelegate) {
				std::cout << "hostfxr_get_runtime_delegate was null" << std::endl;
				return;
			}

			std::cout << "Initializing...starting the process of loading a Assembly" << std::endl;

			//loads a Assembly
			LoadAssembly loadAssembly = nullptr;
			hostfxr_funcPtr_GetRuntimeDelegate(cxt, hdt_load_assembly, (void**)&loadAssembly);
			if (!loadAssembly) {
				std::cout << "Load Assembly not found" << std::endl;

				auto rc = GetLastError();
				std::cout << rc << std::endl;
				return;
			}
			std::cout << "Load Assembly found" << std::endl;

			//gets the C# function
			GetManagedFunctionPointer getManagedFunctionPtr = nullptr;
			hostfxr_funcPtr_GetRuntimeDelegate(cxt, hdt_get_function_pointer, (void**)&getManagedFunctionPtr);
			if (!getManagedFunctionPtr) {
				std::cout << "managedEntryPoint not found" << std::endl;

				auto rc = GetLastError();
				std::cout << rc << std::endl;
				return;
			}
			std::cout << "managedEntryPoint found" << std::endl;
			loadAssembly(assemblyPath, NULL, NULL);

			//calls the C# function
			ManagedEntrypoint managedEntrypoint = nullptr;
			getManagedFunctionPtr(STR("RealLoaderFramework.Program, RealLoaderFramework"), STR("EntryPoint"),
				STR("RealLoaderFramework.Program+VoidDelegateSignature, RealLoaderFramework"), NULL, NULL, (void**)&managedEntrypoint);

			if (!managedEntrypoint) {
				std::cout << "UnmanagedEntrypoint EntryPoint was NULL :'(" << std::endl;

				auto rc = GetLastError();
				std::cout << rc << std::endl;
				return;
			}
			managedEntrypoint();

			hostfxr_funcPtr_Close(cxt); //closes the CLR
		}
	};
}
