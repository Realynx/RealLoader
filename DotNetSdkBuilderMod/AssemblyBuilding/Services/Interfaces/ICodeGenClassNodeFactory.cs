using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.Flags;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenClassNodeFactory {
        CodeGenClassNode GenerateCodeGenClassNode(ClassNode classNode, Dictionary<EClassCastFlags, string> castFlagNames);
    }
}