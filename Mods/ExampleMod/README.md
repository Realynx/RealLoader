# Client Mod Example 🐱‍👤
> [!NOTE]
> This example is for C# Dotnet Core  


## Registering A Mod
To register a dotnet mod using RealLoader, you must attach an attribute to your mod's main class. This class can be named anything. Below is an example of this attribute.

https://github.com/Realynx/RealLoader/blob/6fcb9f537eb43f6b00d2e9ccfe011ea094ddec20/ExampleMod/Sample.cs#L12-L13
  
> [!TIP]
> Mod compatibility allows you to flag on which environments your mod is meant to run on. `Client`, `Server`, `Universal`.

The parameters for this attribute are  
- Your mod's name
- Your contact
- Your alias
- Mod version
- Mod compatibility

## Mod Constructor
A mod may have a parameters-less constructor. We always recommend passing and storing the CancellationToken. You can use this to gracefully shutdown your mod. After cancel has been requested there is a 5 second window to shutdown before a force shutdown occurs.  

https://github.com/Realynx/RealLoader/blob/395843ccff4bc4af3025416036694a8516bfb1c6/ExampleMod/Sample.cs#L20-L24

> [!TIP]
> Even when an ISbStartup class is not specified, a mod may consume any dependency from the root DI service container via the constructor arguments. In this case, ILogger is satisfied by the DI service.

## Loading
Loading and unloading are both called as functions a mod must implement.
### IRealLoaderMod::Load
-   Mod load is a mod's main entry point for initial code execution.

### IRealLoaderMod::Unload
*   Mod unload has a 5 second window before it will force the shutdown of a mod.