using RealLoaderFramework.Sdk.Models.CoreUObject.GNameStructs;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public unsafe sealed class ClassNode {
        public ClassNode? parent;
        public ClassNode[] children;
        public string packageName;

        public UClass* nodeClass;

        public PropertyNode[] properties;
        public FunctionNode[] functions;

        public FNameEntryId ClassName {
            get {
                return nodeClass->baseUStruct.baseUfield.baseUObject.namePrivate.comparisonIndex;
            }
        }
    }

    public unsafe sealed class PropertyNode {
        public FProperty* nodeProperty;
        public PropertyNode? inheritedFrom;

        public FNameEntryId PropertyName {
            get {
                return nodeProperty->ObjectName;
            }
        }
    }

    public unsafe sealed class FunctionNode {
        public UFunction* nodeFunction;
        public FunctionNode? inheritedFrom;

        public FNameEntryId FunctionName {
            get {
                return nodeFunction->baseUstruct.ObjectName;
            }
        }
    }
}
