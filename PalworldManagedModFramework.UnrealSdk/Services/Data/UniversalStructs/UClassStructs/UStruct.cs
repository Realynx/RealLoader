using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {

    [StructLayout(LayoutKind.Sequential, Size = 0xb0)]
    public unsafe struct UStruct {
        public UField baseUfield;
        public FStructBaseChain inheritedFStructBaseChain;

        public UStruct* superStruct;
        public UField* children;
        public FField* childProperties;
        public int propertiesSize;
        public int minAlignment;

        public TArray<byte> script;

        public FProperty* propertyLink;
        public FProperty* refLink;
        public FProperty* destructorLink;
        public FProperty* postConstructLink;

        public TArray<nint> scriptAndPropertyObjectReferences;

        public void* unresolvedScriptProperties;
        public FUnversionedStructSchema* unversionedGameSchema;
    }
}
