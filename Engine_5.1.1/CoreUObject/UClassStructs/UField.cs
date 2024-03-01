using System.Runtime.InteropServices;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/UField/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L133"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public unsafe struct UField {
        [FieldOffset(0x00)]
        public UObjectBase baseUObject;

        [FieldOffset(0x28)]
        public UField* next;
    }
}
