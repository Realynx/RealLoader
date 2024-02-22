using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Sdk.Services.Memory.Windows;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupMemoryScanningServices(this IServiceCollection serviceDescriptors) {

            serviceDescriptors
                .AddSingleton<IOperandResolver, OperandResolver>()
                .AddSingleton<IPropertyManager, PropertyManager>()
                .AddSingleton<ISequenceScanner, SequenceScanner>()
                .AddSingleton<IBulkTypePatternScanner, BulkTypePatternScanner>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                serviceDescriptors
                    .AddSingleton<IProcessSuspender, LinuxProcessSuspender>()
                    .AddSingleton<IMemoryMapper, LinuxMemoryMapper>()
                    .AddSingleton<IMemoryScanner, LinuxMemoryScanner>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                serviceDescriptors
                    .AddSingleton<IProcessSuspender, WindowsProcessSuspender>()
                    .AddSingleton<IMemoryMapper, WindowsMemoryMapper>()
                    .AddSingleton<IMemoryScanner, WindowsMemoryScanner>();
            }

            return serviceDescriptors;
        }
    }
}
