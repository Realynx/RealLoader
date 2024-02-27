using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Linux;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Windows;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
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
