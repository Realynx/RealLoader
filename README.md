# RealLoader
[![Github License](https://img.shields.io/github/license/PoofImaFox/PalworldManagedModFramework.svg)]()
[![CodeFactor](https://www.codefactor.io/repository/github/PoofImaFox/PalworldManagedModFramework/badge)](https://www.codefactor.io/repository/github/PoofImaFox/PalworldManagedModFramework)
  
### Unreal Engine modding tools, framework, and mod orchestration.
- Realloader is a framework for loading, and orchestrating mod's created in various languages for unreal engine.

## Branch Build Status
| Branch | Status |
|--------|--------|
| [Master](/PoofImaFox/PalworldManagedModFramework/tree/master) | [![Build Status](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=master)](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_build/latest?definitionId=1&branchName=master) |
| [Testing](/PoofImaFox/PalworldManagedModFramework/tree/master) | [![Build Status](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=testing)](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_build/latest?definitionId=1&branchName=testing) |
| [Developer](/PoofImaFox/PalworldManagedModFramework/tree/master) | [![Build Status](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_apis/build/status%2FPoofImaFox.PalworldManagedModFramework?branchName=developer)](https://dev.azure.com/PalworldNetCoreMods/Palworld%20Modding%20Framework/_build/latest?definitionId=1&branchName=developer) |



# Building from Source
### Please read the following articles before contributing.
- Contributing
- Code of Conduct
- Editoconfig Style Rules
- Roadmap
  
> [!NOTE]
> The following required dependancies must be met in order to compile from source. Dotnet runtime is required to run the mod framework on every location.  

## Dependancies ![Static Badge](https://img.shields.io/badge/REQUIRED-darkred)
- ### [DotNet Runtime](https://dotnet.microsoft.com/en-us/download/dotnet) (8+).
- ### [CMake](https://cmake.org/download/) (3.22+)
- ### [Visual Studio](https://visualstudio.microsoft.com/downloads/) (2022+)

## How to Build
1. Clone the repo
    - ```bash
        git clone https://github.com/PoofImaFox/PalworldManagedModFramework
        ```
2. Change directory to `Build`  
    - ```bash
        cd ./build
        ```   
        
3.  Execute install script.  

    - ![Static Badge](https://img.shields.io/badge/Windows-blue):  
        - ```bash
            Build.bat
            ```
    - ![Static Badge](https://img.shields.io/badge/Linux-green)  
        - ```bash
            ./Build.sh
            ```
