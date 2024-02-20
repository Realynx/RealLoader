using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public unsafe interface ICodeGenPropertyNodeFactory {
        CodeGenPropertyNode GenerateCodeGenPropertyNode(FProperty* property);
    }
}