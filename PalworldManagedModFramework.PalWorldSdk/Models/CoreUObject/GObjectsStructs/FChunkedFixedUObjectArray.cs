using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.GObjectsStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FChunkedFixedUObjectArray/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UObjectArray.h#L365"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 0x20)]
    public unsafe struct FChunkedFixedUObjectArray {
        /// <summary>
        /// Primary table to chunks of pointers.
        /// </summary>
        public FUObjectItem** objects;

        /// <summary>
        /// If requested, a contiguous memory where all objects are allocated.
        /// </summary>
        public FUObjectItem* preAllocatedObjects;

        /// <summary>
        /// Maximum number of elements.
        /// </summary>
        public int maxElements;

        /// <summary>
        /// Number of elements we currently have.
        /// </summary>
        public int numElements;

        /// <summary>
        /// Maximum number of chunks.
        /// </summary>
        public int maxChunks;

        /// <summary>
        /// Number of chunks we currently have.
        /// </summary>
        public int numChunks;
    }
}
