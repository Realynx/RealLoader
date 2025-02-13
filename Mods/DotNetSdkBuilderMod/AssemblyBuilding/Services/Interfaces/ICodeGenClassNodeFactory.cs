using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenClassNodeFactory {
        CodeGenClassNode GenerateCodeGenClassNode(ClassNode classNode, Dictionary<EClassCastFlags, string> castFlagNames);
        CodeGenClassNode GenerateCustomClass(string name, string baseType);
    }
}