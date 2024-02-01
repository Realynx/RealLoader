cmake_minimum_required(VERSION 3.25) #CMake ver

#option for platforms
option(BUILD_WINDOWS "Performs a build for Windows" OFF)
option(BUILD_LINUX "Performs a build for Linux" OFF)

#sets the project
project(PMMF_OWO)

#adds the sub projects for building
add_subdirectory(CLRHost)
#add_subdirectory(PalWorldManagedModFramework.EntryPoint)