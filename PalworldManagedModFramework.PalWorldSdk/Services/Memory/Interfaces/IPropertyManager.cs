using System.Reflection;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IPropertyManager {
        void UpdatePropertyValue(ByteCodePattern byteCodePattern, nint address, PropertyInfo member, object? instance);
    }
}