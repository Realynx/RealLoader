cmake_minimum_required(VERSION 3.22) # CMake version

# CLR Host
project(CLRHost)

# Set the project
set(CMAKE_SYSTEM_NAME Linux)

# Create the executable
ADD_LIBRARY(${PROJECT_NAME} SHARED
    "src/main.cpp" "src/CLR.hpp" "src/coreclr_delegates.h" "src/hostfxr.h" "src/nethost.h")

# Force it to use C++17
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

# Sets the output name
set_target_properties(${PROJECT_NAME} PROPERTIES OUTPUT_NAME CLRHost)

# Set the include directories
target_include_directories(${PROJECT_NAME} PRIVATE "src" "../Common/Cpp")

# Invoke the bash script to find the correct library path
execute_process(
    COMMAND bash "${CMAKE_SOURCE_DIR}/find_nethost.sh"
    OUTPUT_VARIABLE NETHOST_LIB
    OUTPUT_STRIP_TRAILING_WHITESPACE
)

# Link library
target_link_libraries(${PROJECT_NAME} "${NETHOST_LIB}")

# Defines a Linux Macro for build
add_compile_definitions("__linux__")

# Sets output directory
set_target_properties(${PROJECT_NAME} PROPERTIES LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/GithubSymbols/Linux")

# Use GCC for Linux
set(CMAKE_C_COMPILER "gcc")
set(CMAKE_CXX_COMPILER "g++")
