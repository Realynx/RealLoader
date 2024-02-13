using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public class ClassNode {
        public ClassNode parent;
        public ClassNode[] children;

        public UClass nodeClass;
        public UFunction[] functions;
        public FProperty[] properties;

        public FNameEntryId ClassName {
            get {
                return nodeClass.baseUStruct.baseUfield.baseUObject.namePrivate.comparisonIndex;
            }
        }
    }
}
