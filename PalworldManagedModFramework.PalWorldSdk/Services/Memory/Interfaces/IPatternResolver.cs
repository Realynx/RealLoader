using System.Reflection;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces {
    public interface IPatternResolver {
        nint? ResolvePattern(PropertyInfo member, object? instance = null);
    }
}