using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject.UClassStructs;

namespace Engine_5._1._1.CoreUObject.GObjectsStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FUObjectItem/"/>
    /// |
    /// <see cref="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UObjectArray.h#L26"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct FUObjectItem {
        /// <summary>
        /// Pointer to the allocated object.
        /// </summary>
        public UObjectBase* uObject;

        /// <summary>
        /// Internal flags.
        /// </summary>
        public uint flags;

        /// <summary>
        /// UObject Owner Cluster Index.
        /// </summary>
        public int clusterRootIndex;

        /// <summary>
        /// Weak Object Pointer Serial number associated with the object.
        /// </summary>
        public int serialNumber;


        private int _padding;
    }
}
