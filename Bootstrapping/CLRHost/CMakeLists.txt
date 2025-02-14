cmake_minimum_required(VERSION 3.22) #CMake ver

# CLR Host
project(CLRHost)

# Sets the project
set(CMAKE_SYSTEM_NAME Linux)

# Create the executable
ADD_LIBRARY(${PROJECT_NAME} SHARED
    "src/main.cpp" "src/CLR.hpp" "src/coreclr_delegates.h" "src/hostfxr.h" "src/nethost.h")

# Force it to use C++17
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

# Find the most recent .NETCore.App.Host.linux-x64 version
file(GLOB NETCORE_VERSIONS "/usr/share/dotnet/packs/Microsoft.NETCore.App.Host.linux-x64/*")
list(SORT NETCORE_VERSIONS)
list(REVERSE NETCORE_VERSIONS)
list(GET NETCORE_VERSIONS 0 LATEST_NETCORE)

# Ensure a valid path was found
if(NOT LATEST_NETCORE)
    message(FATAL_ERROR "Could not find Microsoft.NETCore.App.Host.linux-x64 in /usr/share/dotnet/packs/")
endif()

# Set include directories
target_include_directories(${PROJECT_NAME} PRIVATE "src" "../Common/Cpp")

# Link the most recent .NET host library
target_link_libraries(${PROJECT_NAME} "${LATEST_NETCORE}/runtimes/linux-x64/native/libnethost.a")

# Define a Linux macro for build
add_compile_definitions("__linux__")

# Set output directory
set_target_properties(${PROJECT_NAME} PROPERTIES LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin/Linux")

# Use GCC for Linux
set(CMAKE_C_COMPILER "gcc")
set(CMAKE_CXX_COMPILER "g++")
