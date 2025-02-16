# RealLoader is early in development and not fully implemented/functional yet.

[![Github License](https://img.shields.io/github/license/Realynx/RealLoader.svg)](LICENSE.md)
[![CodeFactor](https://www.codefactor.io/repository/github/Realynx/RealLoader/badge)](https://www.codefactor.io/repository/github/Realynx/RealLoader)

| Branch                                                                                | Status                                                                                                                                                                                                                                                 |
|---------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Master](https://github.com/Realynx/RealLoader/tree/master)       | [![Build Status](https://dev.azure.com/RealLoader/RealLoader%20Development/_apis/build/status%2FRealynx.RealLoader?branchName=master)](https://dev.azure.com/RealLoader/RealLoader%20Development/_build/latest?definitionId=2&branchName=master)       |
| [Testing](https://github.com/Realynx/RealLoader/tree/testing)     | [![Build Status](https://dev.azure.com/RealLoader/RealLoader%20Development/_apis/build/status%2FRealynx.RealLoader?branchName=testing)](https://dev.azure.com/RealLoader/RealLoader%20Development/_build/latest?definitionId=2&branchName=testing)     |
| [Poofy Feature Branch](https://github.com/Realynx/RealLoader/tree/poofyfeatures) | [![Build Status](https://dev.azure.com/RealLoader/RealLoader%20Development/_apis/build/status%2FRealynx.RealLoader?branchName=PoofyFeatures)](https://dev.azure.com/RealLoader/RealLoader%20Development/_build/latest?definitionId=2&branchName=PoofyFeatures) |


# **RealLoader** 🎷🐛
> [!TIP]
>Remember to launch your game with -modded flag (usualy done in steam), otherwise the mod framework will run in bypass-mode allowing you to play un modded by default.

### Unreal Engine Modding Framework
 
**RealLoader** is a powerful framework for loading and orchestrating mods created in various languages for **Unreal Engine**. It primarily supports **C#** for developing mods and provides a streamlined modding SDK with essential tools.

## **Key Features**

- **Multi-Language Mod Support**  
  - Primarily designed for **C# mod development**.  
  - Provides extensibility for integrating mods written in other languages.  

- **Function Hooking & Unreal Engine API Calls**  
  - Allows mods to **hook into Unreal Engine functions** dynamically.  
  - Enables calling Unreal Engine functions by name for seamless interaction.  

- **Source-Generated SDK for Installed Games**  
  - Automatically generates a **full SDK** of the game RealLoader is installed to.  
  - Ensures modders have access to the necessary game-specific APIs.  

- **Dependency Injection for C# Mods**  
  - Handles mod dependencies efficiently using **DI (Dependency Injection)**.  
  - Allows for modular and scalable mod development.  

- **Mod Orchestration & Management**  
  - Provides a **framework for managing mods**, ensuring compatibility and execution order.  
  - Supports enabling, disabling, and configuring mods dynamically.  


# Building from Source
> [!NOTE]
> The following required dependencies must be met in order to compile from source. Dotnet runtime is required to run the mod framework on every location.

### Dependencies
- [.NET 9+ Runtime](https://dotnet.microsoft.com/en-us/download/dotnet)

### How to Build
> [!TIP]
> All build symbols are configured to go to `/bin` in the root directory.

1. Clone the repo
    ```bash
    git clone https://github.com/Realynx/RealLoader
    ```
---
- ![Windows Badge](https://img.shields.io/badge/Windows-blue)
1. Install [Visual Studio 2022+](https://visualstudio.microsoft.com/downloads/) with the <b>Desktop development with C++</b> package.  
You can install this using the visual studio installer  
For Cli-only Based Builder (like windows core) you can use these installers from [Installer Packages Page](https://learn.microsoft.com/en-us/visualstudio/install/use-command-line-parameters-to-install-visual-studio?view=vs-2022) or use [Direct Link](https://aka.ms/vs/17/release/vs_buildtools.exe)
    ```
    .\vs_buildtools.exe --add Microsoft.VisualStudio.Workload.VCTools --includeRecommended --quiet --wait
    ```
2. Open the solution in IDE and build.   

---
- ![Linux Badge](https://img.shields.io/badge/Linux-green)
1. Install the [.NET 9+ SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
2. Install [CMake 3.22+](https://cmake.org/download/)
3. Run Cmake from root directory.
    ```sh
    cmake ./
    cmake --build ./Build
    ```

# Resources
> [!TIP]
> You can view example mods under [Client Mod Ex](Mods/ExampleMod), [Server Mod Ex](Mods/ExampleServerMod), and [DI Mod Ex](Mods)  

## Please read the following articles before contributing.

- Contributing
- Code of Conduct
- Editorconfig Style Rules
- Roadmap