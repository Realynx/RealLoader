using System.Runtime.InteropServices;

namespace Engine_5._1._1.CoreUObject.GObjectsStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FUObjectArray/"/>
    /// |
    /// <see cref="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UObjectArray.h#L579"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0xb8)]
    public unsafe struct FUObjectArray {
        // starts here: https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UObjectArray.h#L1075

        /// <summary>
        /// First index into objects array taken into account for GC.
        /// </summary>
        public int objFirstGCIndex;

        /// <summary>
        /// Index pointing to last object created in range disregarded for GC.
        /// </summary>
        public int objLastNonGCIndex;

        /// <summary>
        /// Maximum number of objects in the disregard for GC Pool
        /// </summary>
        public int MaxObjectsNotConsideredByGC;

        /// <summary>
        /// If true this is the intial load and we should load objects int the disregarded for GC range.
        /// </summary>
        public bool OpenForDisregardForGC;

        /// <summary>
        /// Array of all live objects.
        /// </summary>
        public FChunkedFixedUObjectArray ObjObjects;


    }
}
