using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Structs {
    public class GObjectsStructs {
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

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public unsafe struct FUObjectItem {
            public UObjectBase* uObject;
            public uint flags;
            public int clusterRootIndex;
            public int serialNumber;
            public int _padding;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct UObjectBase {
            public void* vptrUObjectBase;
            public uint objectFlags;
            public uint internalIndex;
            public void* classPrivate;
            public GNamesStructs.FName namePrivate;
            public void* outerPrivate;
        }
    }
}
