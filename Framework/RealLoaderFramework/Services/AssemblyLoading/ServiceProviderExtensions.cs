using Microsoft.Extensions.DependencyInjection;

using RealLoaderFramework.Services.AssemblyLoading.Interfaces;

namespace RealLoaderFramework.Services.AssemblyLoading {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupReflectionModLoader(this IServiceCollection serviceDescriptors) {
            serviceDescriptors
                .AddSingleton<IAssemblyDiscovery, AssemblyDiscovery>()
                .AddSingleton<IModLoader, ModLoader>();

            return serviceDescriptors;
        }
    }
}
