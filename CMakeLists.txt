cmake_minimum_required(VERSION 3.12)

#CLR Host
project(CLRHost)

# Create the executable
add_executable(${PROJECT_NAME}
    "CLRHost/src/main.cpp" "CLRHost/src/CLR.hpp" "CLRHost/src/coreclr_delegates.h" "CLRHost/src/hostfxr.h")
    
#force it to use C++17
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

#set the include directories
target_include_directories(${PROJECT_NAME} PRIVATE "CLRHost/src"
"/usr/share/dotnet/packs/Microsoft.NETCore.App.Host.linux-x64/8.0.1/runtimes/linux-x64/native" 
"Common/Cpp")

#link library
target_link_libraries(${PROJECT_NAME} 
"/usr/share/dotnet/packs/Microsoft.NETCore.App.Host.linux-x64/8.0.1/runtimes/linux-x64/native/libnethost.a")

#defines a Linux Macro for build
add_compile_definitions("__linux__")

# Set up the different compilers for different platforms
#if(WIN32)
#    # Use MinGW for Windows
#    set(CMAKE_C_COMPILER "gcc")
#    set(CMAKE_CXX_COMPILER "g++")
#elseif(UNIX)
# Use GCC for Linux
set(CMAKE_C_COMPILER "gcc")
set(CMAKE_CXX_COMPILER "g++")
#endif()