using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;
using RealLoaderFramework.Sdk.Services.Detour.AssemblerServices;
using RealLoaderFramework.Sdk.Services.Detour.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RealLoaderFramework.Sdk.Services.Memory.Linux;
using RealLoaderFramework.Sdk.Services.Memory.Windows;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;

namespace RealLoaderFramework.Sdk.Services.Detour {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupDetourServices(this IServiceCollection serviceDescriptors) {
            serviceDescriptors
                .AddSingleton<IStackDetourService, StackDetourService>()
                .AddSingleton<IInstructionPatcher, InstructionPatcher>()
                .AddSingleton<IDetourManager, DetourManager>()
                .AddSingleton<IDetourAttributeService, DetourAttributeService>()
                .AddSingleton<IShellCodeReader, ShellCodeReader>()
                .AddSingleton<IDetourRegistrationService, DetourRegistrationService>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                serviceDescriptors
                    .AddSingleton<IMemoryAllocate, LinuxMemoryAllocate>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                serviceDescriptors
                    .AddSingleton<IMemoryAllocate, WindowsMemoryAllocate>();
            }

            return serviceDescriptors;
        }
    }
}
