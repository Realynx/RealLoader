using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.FunctionServices;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
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

            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                serviceDescriptors
                    .AddSingleton<IEnginePattern, LinuxServerPattern>();
                //.AddSingleton<IUObjectFuncs, LinuxUObjectFuncs>()
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                serviceDescriptors
                    .AddSingleton<IEnginePattern, WindowsClientPattern>()
                    .AddSingleton<IUObjectFuncs, WindowsUObjectFuncs>();
            }

            return serviceDescriptors;
        }
    }
}
