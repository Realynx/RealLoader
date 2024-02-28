using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public unsafe interface ICodeGenMethodNodeFactory {
        CodeGenMethodNode GenerateCodeGenMethodNode(UFunction* method, Index methodIndex);
        CodeGenMethodNode GenerateInheritedMethod(UFunction* method);
    }
}