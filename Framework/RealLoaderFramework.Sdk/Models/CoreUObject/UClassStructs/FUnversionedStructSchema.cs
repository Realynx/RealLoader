using System.Runtime.InteropServices;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see cref="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Private/Serialization/UnversionedPropertySerialization.cpp#L285"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FUnversionedStructSchema {
        public uint num;
        public FUnversionedPropertySerializer* Serializers;
    }
}
