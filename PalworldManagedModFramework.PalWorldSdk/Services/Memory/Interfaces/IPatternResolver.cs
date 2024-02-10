using System.Reflection;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IPatternResolver {
        nint? ResolvePattern(PropertyInfo member, object? instance = null);
    }
}