# PalworldManagedModFramework

A framework for loading mods created with C++ and C# for Pal World
<br>

# How to Build

-Required
Regardless of the platform DotNet SDK 8 is required for compilation.

- Linux
In the root dir, run `cmake . && cmake --build .`

Optionly when calling `cmake .` you can pass a `-dotnet-sdk="path"` after the . to override the default for where dotnet is located on your system.

Linux default: /usr/share/dotnet

(Note: The Bootstrapper is not needed for Linux Servers, only the CLR Host)

- Windows