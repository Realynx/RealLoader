# RealLoader 🎷🐛

[![Github License](https://img.shields.io/github/license/PoofImaFox/RealLoader.svg)](LICENSE.md)
[![CodeFactor](https://www.codefactor.io/repository/github/PoofImaFox/RealLoader/badge)](https://www.codefactor.io/repository/github/PoofImaFox/RealLoader)

### Unreal Engine modding tools, framework, and mod orchestration.

- RealLoader is a framework for loading, and orchestrating mod's created in various languages for unreal engine.

## Branch Build Status

| Branch                                                                                | Status                                                                                                                                                                                                                                                                                                   |
|---------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Master](https://github.com/PoofImaFox/RealLoader/tree/master)       | [![Build Status](https://dev.azure.com/RealLoader/RealLoader%20Development/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=master)](https://dev.azure.com/RealLoader/RealLoader%20Development/_build/latest?definitionId=1&branchName=master)       |
| [Testing](https://github.com/PoofImaFox/RealLoader/tree/testing)     | [![Build Status](https://dev.azure.com/RealLoader/RealLoader%20Development/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=testing)](https://dev.azure.com/RealLoader/RealLoader%20Development/_build/latest?definitionId=1&branchName=testing)     |
| [Developer](https://github.com/PoofImaFox/RealLoader/tree/developer) | [![Build Status](https://dev.azure.com/RealLoader/RealLoader%20Development/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=developer)](https://dev.azure.com/RealLoader/RealLoader%20Development/_build/latest?definitionId=1&branchName=developer) |

# Building from Source

### Please read the following articles before contributing.

- Contributing
- Code of Conduct
- Editorconfig Style Rules
- Roadmap

> [!NOTE]
> The following required dependencies must be met in order to compile from source. Dotnet runtime is required to run the mod framework on every location.

## Dependencies

- ### [.NET 8+ Runtime](https://dotnet.microsoft.com/en-us/download/dotnet)

## How to Build

1. Install the [.NET 8+ SDK](https://dotnet.microsoft.com/en-us/download/dotnet)

2. Install [CMake 3.22+](https://cmake.org/download/)
 
3. Install [Visual Studio 2022+](https://visualstudio.microsoft.com/downloads/) with the <b>Desktop development with C++</b> package

4. Clone the repo
    ```bash
    git clone https://github.com/PoofImaFox/RealLoader
    ```
5. Change directory to `Build`
    ```sh
    cd ./Build
    ```

6. Run the build script

    - ![Windows Badge](https://img.shields.io/badge/Windows-blue)
         ```sh
         Build.bat
         ```
    - ![Linux Badge](https://img.shields.io/badge/Linux-green)
        ```sh
        ./Build.sh
        ```


# Resources
> [!TIP]
> You can view example mods under [Client Mod Ex](Mods/ExampleMod), [Server Mod Ex](Mods/ExampleServerMod), and [DI Mod Ex](Mods)  
