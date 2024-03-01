using System.Runtime.InteropServices;

namespace Engine_5._1._1.CoreUObject.GNameStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/Core/UObject/FNameEntryId/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L50C1-L50C20"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FNameEntryId {
        ///// <summary>
        ///// <see cref="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L107"/>
        ///// </summary>
        public uint value;

        public ushort LowerOrderValue {
            get {
                return (ushort)(value & 0xffff);
            }
        }
        public ushort HigherOrderValue {
            get {
                return (ushort)(value >> 16);
            }
        }
    }
}
