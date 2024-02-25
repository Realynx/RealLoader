using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Interfaces {
    public interface IDetourAttributeService {
        DetourAttribute? FindDetourAttribute(MethodInfo detourMethod);
        ManagedDetourInfo? GetManagedDetourInfo(MethodInfo detourMethod);
    }
}