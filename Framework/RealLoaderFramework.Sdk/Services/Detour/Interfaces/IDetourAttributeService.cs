using System.Reflection;

using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Services.Detour.Models;

namespace RealLoaderFramework.Sdk.Services.Detour.Interfaces {
    public interface IDetourAttributeService {
        DetourAttribute? FindDetourAttribute(MethodInfo detourMethod);
        ManagedDetourInfo? GetManagedDetourInfo(MethodInfo detourMethod);
    }
}