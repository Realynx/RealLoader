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
        hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr;
        hostfxr_initialize_for_runtime_config_fn init_for_config_fptr;
        hostfxr_get_runtime_delegate_fn get_delegate_fptr;
        hostfxr_run_app_fn run_app_fptr;
        hostfxr_close_fn close_fptr;
        hostfxr_handle cxt = nullptr;

        //inits the CLR Host
        inline bool Init(const char_t* assembly_path)
        {
            get_hostfxr_parameters params{ sizeof(get_hostfxr_parameters), assembly_path, nullptr };
            // Pre-allocate a large buffer for the path to hostfxr
            char_t buffer[MAX_PATH];
            size_t buffer_size = sizeof(buffer) / sizeof(char_t);
            int rc = get_hostfxr_path(buffer, &buffer_size, &params);
            if (rc != 0)
                return false;

            // Load hostfxr and get desired exports
            void* lib = Util::load_library(buffer);
            init_for_cmd_line_fptr = (hostfxr_initialize_for_dotnet_command_line_fn)Util::get_export(lib, "hostfxr_initialize_for_dotnet_command_line");
            init_for_config_fptr = (hostfxr_initialize_for_runtime_config_fn)Util::get_export(lib, "hostfxr_initialize_for_runtime_config");
            get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)Util::get_export(lib, "hostfxr_get_runtime_delegate");
            run_app_fptr = (hostfxr_run_app_fn)Util::get_export(lib, "hostfxr_run_app");
            close_fptr = (hostfxr_close_fn)Util::get_export(lib, "hostfxr_close");

            return (init_for_config_fptr && get_delegate_fptr && close_fptr);
        }

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
    };
}