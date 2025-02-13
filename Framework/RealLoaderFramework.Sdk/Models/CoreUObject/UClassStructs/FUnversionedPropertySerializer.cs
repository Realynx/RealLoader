using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
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
