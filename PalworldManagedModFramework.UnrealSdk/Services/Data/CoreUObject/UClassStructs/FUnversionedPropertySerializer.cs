using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Private/Serialization/UnversionedPropertySerialization.cpp#L44C7-L44C37"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FUnversionedPropertySerializer {
        /// <summary>
        /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Private/Serialization/UnversionedPropertySerialization.cpp#L271"/>
        /// </summary>
        public FProperty* property;

        public uint offset;
        public bool bSerializeAsInteger;
        public EIntegerType IntType;
        public byte fastZeroIntNum;
    }
}
