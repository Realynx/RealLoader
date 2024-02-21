using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;

namespace PalworldManagedModFramework.Sdk.Services.UnrealHook.Interfaces {
    public interface IUnrealHookManager {
        IUnrealHookManager RegisterUnrealEvent(MethodInfo engineEventMethod, object instance);
        IUnrealHookManager RegisterUnrealHook(MethodInfo hookEngineEventMethod, object instance);
    }
}