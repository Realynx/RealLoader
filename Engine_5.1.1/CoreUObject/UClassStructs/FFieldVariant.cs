using System.Runtime.InteropServices;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href=""/> (Does not exist in docs?)
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Field.h#L262"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x10, Pack = 8)]
    public struct FFieldVariant {
        [FieldOffset(0)]
        public FFieldObjectUnion Container;

        [FieldOffset(8)]
        public bool bIsUObject;
    }
}
