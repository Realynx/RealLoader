using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Linux;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Windows;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Sdk.Services.Memory.Windows;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupBasicUnrealEngineServices(this IServiceCollection serviceDescriptors) {

            serviceDescriptors
                .AddSingleton<IGlobalObjectsTracker, GlobalObjectsTracker>()
                .AddSingleton<IUnrealReflection, UnrealReflection>()
                .AddSingleton<IGlobalObjects, GlobalObjects>()
                .AddSingleton<INamePoolService, NamePoolService>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                serviceDescriptors
                    .AddSingleton<IEnginePattern, LinuxServerPattern>()
                    .AddSingleton<IUObjectFuncs, LinuxUObjectFuncs>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                serviceDescriptors
                    .AddSingleton<IEnginePattern, WindowsClientPattern>()
                    .AddSingleton<IUObjectFuncs, WindowsUObjectFuncs>();
            }

            return serviceDescriptors;
        }
    }
}
