using PalworldManagedModFramework.Sdk.Models.CoreUObject.GNameStructs;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public unsafe class ClassNode {
        public ClassNode parent;
        public ClassNode[] children;
        public string packageName;

        public UClass* nodeClass;
        public UFunction*[] functions;
        public FProperty*[] properties;

        public FNameEntryId ClassName {
            get {
                return nodeClass->baseUStruct.baseUfield.baseUObject.namePrivate.comparisonIndex;
            }
        }
    }
}
