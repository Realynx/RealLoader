using System.Reflection;

using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface IPropertyManager {
        void UpdatePropertyValue(ByteCodePattern byteCodePattern, nint address, PropertyInfo member, object? instance);
    }
}