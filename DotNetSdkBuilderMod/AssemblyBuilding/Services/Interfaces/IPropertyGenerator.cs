using System.Text;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IPropertyGenerator {
        void GenerateProperty(StringBuilder codeBuilder, FProperty property);
    }
}