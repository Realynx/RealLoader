cmake_minimum_required(VERSION 3.22) #CMake ver

#CLR Host
project(CLRHost)

#sets the project
set(CMAKE_SYSTEM_NAME Linux)

# Create the executable
ADD_LIBRARY( ${PROJECT_NAME} SHARED
    "src/main.cpp" "src/CLR.hpp" "src/coreclr_delegates.h" "src/hostfxr.h" "src/nethost.h")
    
#force it to use C++17
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

#sets the output name
set_target_properties(${PROJECT_NAME} PROPERTIES OUTPUT_NAME CLRHost)

#set the include directories
target_include_directories(${PROJECT_NAME} PRIVATE "src"
#"/home/DotNet/packs/Microsoft.NETCore.App.Host.linux-x64/8.0.1/runtimes/linux-x64/native" 
"../Common/Cpp")

#link library
target_link_libraries(${PROJECT_NAME} 
"/usr/share/dotnet/packs/Microsoft.NETCore.App.Host.linux-x64/8.0.1/runtimes/linux-x64/native/libnethost.a")

#defines a Linux Macro for build
add_compile_definitions("__linux__")

#sets output directory
set_target_properties(${PROJECT_NAME} PROPERTIES LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/GithubSymbols/Linux")

# Use GCC for Linux
set(CMAKE_C_COMPILER "gcc")
set(CMAKE_CXX_COMPILER "g++")