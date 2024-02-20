using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenClassNodeFactory {
        CodeGenClassNode GenerateCodeGenClassNode(ClassNode classNode, Dictionary<EClassCastFlags, string> castFlagNames);
    }
}