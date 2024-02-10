using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GobjectsStructs {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct FUObjectArray {
        public int objFirstGCIndex;
        public int objLastNonGCIndex;
        public int MaxObjectsNotConsideredByGC;
        public bool OpenForDisregardForGC;

        public FUObjectItem** objects;
        public FUObjectItem* preAllocatedObjects;
        public int maxElements;
        public int numElements;
        public int maxChunks;
        public int numChunks;
    }
}
