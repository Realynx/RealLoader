using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Services.SandboxDI.Interfaces;

namespace PalworldManagedModFramework.Services.SandboxDI {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupSandboxedDIServices(this IServiceCollection serviceDescriptors) {
            serviceDescriptors
                .AddSingleton<ISandboxDIService, SandboxDIService>();

            return serviceDescriptors;
        }
    }
}
