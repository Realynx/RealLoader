using Microsoft.Extensions.DependencyInjection;

using RealLoaderFramework.Services.SandboxDI.Interfaces;

namespace RealLoaderFramework.Services.SandboxDI {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupSandboxedDIServices(this IServiceCollection serviceDescriptors) {
            serviceDescriptors
                .AddSingleton<ISandboxDIService, SandboxDIService>();

            return serviceDescriptors;
        }
    }
}
