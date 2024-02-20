using System.Reflection;

using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Interfaces {
    public interface IDetourAttributeScanner {
        ManagedDetourInfo[] FindAllDetourInfos(IEnumerable<Assembly> assemblies);
        ManagedDetourInfo? FindDetourInformation(MethodInfo detourMethod);
    }
}