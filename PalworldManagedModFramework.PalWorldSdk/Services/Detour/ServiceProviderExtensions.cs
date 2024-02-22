using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Sdk.Services.Memory.Windows;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupDetourServices(this IServiceCollection serviceDescriptors) {
            serviceDescriptors
                .AddSingleton<IStackDetourService, StackDetourService>()
                .AddSingleton<IInstructionPatcher, InstructionPatcher>()
                .AddSingleton<IDetourManager, DetourManager>()
                .AddSingleton<IDetourAttributeScanner, DetourAttributeScanner>()
                .AddSingleton<IShellCodeReader, ShellCodeReader>();

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
