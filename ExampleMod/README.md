# Client Mod Example 🐱‍👤
> [!NOTE]
> This example is for C# Dotnet Core  


## Registering Your Mod
To register a dotnet mod using RealLoader, you must attach an attribute to your mod's main class. This class can be named anything. Below is an example of this attribute.

https://github.com/PoofImaFox/PalworldManagedModFramework/blob/6fcb9f537eb43f6b00d2e9ccfe011ea094ddec20/ExampleMod/Sample.cs#L12
  
> [!TIP]
> Mod compatibility allows you to flag on which environments your mod is meant to run on. `Client`, `Server`, `Universal`.

The parameters for this attribute are  
- Your mod's name
- Your contact
- Your alias
- Mod version
- Mod compatibility

## Mod Constructor
Your mod may have a parameters-less constructor. We always recommend passing and storing the CancellationToken. You can use this to gracefully shutdown your mod. After cancel has been requested there is a 5 second window to shutdown before a force shutdown occurs.  

https://github.com/PoofImaFox/PalworldManagedModFramework/blob/43bd84b9f30d9bd12649cb0fab93d6888b8d5e9d/ExampleMod/Sample.cs#L20C1-L24C10

