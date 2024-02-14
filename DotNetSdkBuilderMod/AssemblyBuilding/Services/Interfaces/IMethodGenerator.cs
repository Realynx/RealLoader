using System.Text;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IMethodGenerator {
        void GenerateMethod(StringBuilder codeBuilder, UFunction method);
    }
}