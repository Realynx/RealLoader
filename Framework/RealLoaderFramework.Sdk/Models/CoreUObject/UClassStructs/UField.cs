using System.Runtime.InteropServices;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/UField/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L133"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 0x30)]
    public unsafe struct UField {
        public UObjectBase baseUObject;

        public UField* next;
    }
}
