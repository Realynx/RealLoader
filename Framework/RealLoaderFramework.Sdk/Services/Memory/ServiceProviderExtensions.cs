using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Linux;
using RealLoaderFramework.Sdk.Services.Memory.Windows;

namespace RealLoaderFramework.Sdk.Services.Memory {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupMemoryScanningServices(this IServiceCollection serviceDescriptors) {

            serviceDescriptors
                .AddSingleton<IOperandResolver, OperandResolver>()
                .AddSingleton<IPropertyManager, PropertyManager>()
                .AddSingleton<ISequenceScanner, SequenceScanner>()
                .AddSingleton<IBulkPatternScanner, BulkPatternScanner>()
                .AddSingleton<IStackWalker, StackWalker>();

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
