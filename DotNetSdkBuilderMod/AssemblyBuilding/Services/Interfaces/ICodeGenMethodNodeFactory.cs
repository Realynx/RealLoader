using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public unsafe interface ICodeGenMethodNodeFactory {
        CodeGenMethodNode GenerateCodeGenMethodNode(UFunction* method, Index methodIndex);
    }
}