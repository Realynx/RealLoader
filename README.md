# PalworldManagedModFramework

A framework for loading mods created with C++ and C# for Pal World
<br>

# How to Build

![Static Badge](https://img.shields.io/badge/REQUIRED-darkred)

Regardless of the platform [DotNet SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) is required for compilation.

[CMake](https://cmake.org/download/) (min ver of 3.22) is required for compilation on Linux.

[Visual Studio](https://visualstudio.microsoft.com/downloads/) (2022 or beyond is recommended) is required for compilation on Windows 10.

![Static Badge](https://img.shields.io/badge/Linux-orange)<br>
In the root dir, run the command `cmake . && cmake --build .`

Optionly when calling `cmake .` you can pass a `-dotnet-sdk="path"` after the . to override the default for where dotnet is located on your system.

![Static Badge](https://img.shields.io/badge/Linux_default_Path_for_Dotnet_SDK:-orange) ```/usr/share/dotnet```


(Note: The Bootstrapper is not needed for Linux Servers, only the CLR Host)

![Static Badge](https://img.shields.io/badge/Windows-blue)<br>
In the root dir, run `BuildProjects.bat`, it will compile distribution builds for the Bootstrapper and CLRHost and place them in ```GithubSymbols\Windows```. DotNet assemblies will be generated and placed in ```GithubSymbols\Net```

![Static Badge](https://img.shields.io/badge/Windows_default_Path_for_Dotnet_SDK:-abcdef) `C:\\ProgramFiles\\dotnet`