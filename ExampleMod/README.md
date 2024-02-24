# Client Mod Example 🐱‍👤

> [!NOTE]
> This example is for C# Dotnet Core  

To register a dotnet mod using RealLoader, you must attach an attribute to your mod's main class. This class can be named anything.

https://github.com/PoofImaFox/PalworldManagedModFramework/blob/6fcb9f537eb43f6b00d2e9ccfe011ea094ddec20/ExampleMod/Sample.cs#L12
  
The parameters for this attribute are  
- Your mod's name
- Your contact
- Your alias
- Mod version
- Mod compatibility

> [!TIP]
> Mod compatibility allows you to flag on which environments your mod is meant to run on. `Client`, `Server`, `Universal`.