using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;
using RealLoaderFramework.Sdk.Services.EngineServices.Linux;
using RealLoaderFramework.Sdk.Services.EngineServices.Windows;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;

namespace RealLoaderFramework.Sdk.Services.EngineServices {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupBasicUnrealEngineServices(this IServiceCollection serviceDescriptors) {

            serviceDescriptors
                .AddSingleton<IGlobalObjectsTracker, GlobalObjectsTracker>()
                .AddSingleton<IUnrealReflection, UnrealReflection>()
                .AddSingleton<INamePoolService, NamePoolService>()
                .AddSingleton<IPropertyRegistrationService, PropertyRegistrationService>()
                .AddSingleton<IGlobalObjects, GlobalObjects>()
                .AddSingleton<IUnrealHookManager, UnrealHookManager>()
                .AddSingleton<IUnrealEventRegistrationService, UnrealEventRegistrationService>()
                .AddSingleton<UObjectFactory>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                serviceDescriptors
                    .AddSingleton<IEnginePattern, LinuxEnginePattern>()
                    .AddSingleton<IUObjectFuncs, LinuxUObjectFuncs>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                serviceDescriptors
                    .AddSingleton<IEnginePattern, WindowsEnginePattern>()
                    .AddSingleton<IUObjectFuncs, WindowsUObjectFuncs>();
            }

            return serviceDescriptors;
        }
    }
}
