cmake_minimum_required(VERSION 3.12) #CMake ver

#CLR Host
project(CLRHost)

# Create the executable
ADD_LIBRARY( ${PROJECT_NAME} SHARED
    "src/main.cpp" "src/CLR.hpp" "src/coreclr_delegates.h" "src/hostfxr.h")
    
#force it to use C++17
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

#sets the output name
set_target_properties(${PROJECT_NAME} PROPERTIES OUTPUT_NAME CLRHost)

#if it's Windows
if(BUILD_WINDOWS)

    #set the include directories
    target_include_directories(${PROJECT_NAME} PRIVATE "src"
    "C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Host.win-x64\\8.0.1\\runtimes\\win-x64\\native" 
    "Common/Cpp")

    #link library
    target_link_libraries(${PROJECT_NAME} 
    "C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Host.win-x64\\8.0.1\\runtimes\\win-x64\\native\\libnethost.lib"
    "kernal32.lib" "user32.lib" "gdi32.lib" "winspool.lib" "comdlg32.lib" "advapi32.lib" "shell32.lib" "oleaut32.lib" "uuid.lib" "odbccp32.lib")

    #defines a Linux Macro for build
    add_compile_definitions("__WIN32")

    #sets output directory
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "GithubSymbols/Windows")

    # Use GCC for Linux
    set(CMAKE_C_COMPILER "gcc")
    set(CMAKE_CXX_COMPILER "g++")
    
endif()

#if it's Linux
if(BUILD_LINUX)

    #set the include directories
    target_include_directories(${PROJECT_NAME} PRIVATE "src"
    "/usr/share/dotnet/packs/Microsoft.NETCore.App.Host.linux-x64/8.0.1/runtimes/linux-x64/native" 
    "Common/Cpp")

    #link library
    target_link_libraries(${PROJECT_NAME} 
    "/usr/share/dotnet/packs/Microsoft.NETCore.App.Host.linux-x64/8.0.1/runtimes/linux-x64/native/libnethost.a")

    #defines a Linux Macro for build
    add_compile_definitions("__linux__")

    #sets output directory
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "GithubSymbols/Linux")

    # Use GCC for Linux
    set(CMAKE_C_COMPILER "gcc")
    set(CMAKE_CXX_COMPILER "g++")

endif()