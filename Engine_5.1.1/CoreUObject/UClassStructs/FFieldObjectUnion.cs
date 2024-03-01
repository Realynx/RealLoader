using System.Runtime.InteropServices;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Field.h#L264"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct FFieldObjectUnion {
        [FieldOffset(0)]
        public FField* Field;

        [FieldOffset(0)]
        public UObject* Object;
    }
}
