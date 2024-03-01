using System.Reflection;

using RealLoaderFramework.Sdk.Attributes;

namespace RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces {
    public interface IUnrealHookManager {
        IUnrealHookManager RegisterUnrealEvent(MethodInfo engineEventMethod, object instance);
        IUnrealHookManager RegisterUnrealHook(MethodInfo hookEngineEventMethod, object instance);
    }
}