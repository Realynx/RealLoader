using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public unsafe interface ICodeGenPropertyNodeFactory {
        CodeGenPropertyNode GenerateCodeGenPropertyNode(FProperty* property);
    }
}