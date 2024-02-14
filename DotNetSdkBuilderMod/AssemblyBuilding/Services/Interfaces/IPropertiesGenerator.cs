using System.Text;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public interface IPropertiesGenerator {
        void GenerateProperty(StringBuilder codeBuilder, FProperty property);
    }
}