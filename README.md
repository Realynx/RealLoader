# RealLoader

[![Github License](https://img.shields.io/github/license/PoofImaFox/PalworldManagedModFramework.svg)](LICENSE.md)
[![CodeFactor](https://www.codefactor.io/repository/github/PoofImaFox/PalworldManagedModFramework/badge)](https://www.codefactor.io/repository/github/PoofImaFox/PalworldManagedModFramework)

### Unreal Engine modding tools, framework, and mod orchestration.

- RealLoader is a framework for loading, and orchestrating mod's created in various languages for unreal engine.

## Branch Build Status

| Branch                                                                                | Status                                                                                                                                                                                                                                                                                                   |
|---------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Master](https://github.com/PoofImaFox/PalworldManagedModFramework/tree/master)       | [![Build Status](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=master)](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_build/latest?definitionId=1&branchName=master)       |
| [Testing](https://github.com/PoofImaFox/PalworldManagedModFramework/tree/testing)     | [![Build Status](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=testing)](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_build/latest?definitionId=1&branchName=testing)     |
| [Developer](https://github.com/PoofImaFox/PalworldManagedModFramework/tree/developer) | [![Build Status](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=developer)](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_build/latest?definitionId=1&branchName=developer) |

# Building from Source

### Please read the following articles before contributing.

- Contributing
- Code of Conduct
- Editorconfig Style Rules
- Roadmap

> [!NOTE]
> The following required dependencies must be met in order to compile from source. Dotnet runtime is required to run the mod framework on every location.

## Dependencies ![Required Badge](https://img.shields.io/badge/REQUIRED-darkred)

- ### [DotNet Runtime](https://dotnet.microsoft.com/en-us/download/dotnet) (8+)
- ### [CMake](https://cmake.org/download/) (3.22+)
- ### [Visual Studio](https://visualstudio.microsoft.com/downloads/) (2022+)

## How to Build

1. Clone the repo
    ```bash
    git clone https://github.com/PoofImaFox/PalworldManagedModFramework
    ```
2. Change directory to `Build`
    ```sh
    cd ./Build
    ```

3. Execute install script.

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
> You can view example mods under [Client Mod Ex](ExampleMod), [Server Mod Ex](ExampleMod), and [DI Mod Ex](ExampleMod)  
