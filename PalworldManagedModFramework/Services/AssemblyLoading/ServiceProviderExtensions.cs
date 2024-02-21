using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;

namespace PalworldManagedModFramework.Services.AssemblyLoading {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupReflectionModLoader(this IServiceCollection serviceDescriptors) {
            serviceDescriptors
                .AddSingleton<IAssemblyDiscovery, AssemblyDiscovery>()
                .AddSingleton<IModLoader, ModLoader>();

            return serviceDescriptors;
        }
    }
}
