using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Interfaces {
    public interface IDetourAttributeScanner {
        ManagedDetourInfo[] FindAllDetourInfos(IEnumerable<Assembly> assemblies);
        DetourAttribute FindDetourAttribute(MethodInfo detourMethod);
        ManagedDetourInfo? FindDetourInformation(MethodInfo detourMethod);
    }
}