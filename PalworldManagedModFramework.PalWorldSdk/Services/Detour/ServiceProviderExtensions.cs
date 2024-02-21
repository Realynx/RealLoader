using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using PalworldManagedModFramework.Sdk.Services.Detour.Linux;
using PalworldManagedModFramework.Sdk.Services.Detour.Windows;

namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public static class ServiceProviderExtensions {
        public static IServiceCollection SetupDetourServices(this IServiceCollection serviceDescriptors) {
            serviceDescriptors
                .AddSingleton<IStackDetourService, StackDetourService>()
                .AddSingleton<IInstructionPatcher, InstructionPatcher>()
                .AddSingleton<IDetourManager, DetourManager>()
                .AddSingleton<IDetourAttributeScanner, DetourAttributeScanner>()
                .AddSingleton<IShellCodeReader, ShellCodeReader>();

            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                serviceDescriptors
                    .AddSingleton<IMemoryAllocate, LinuxMemoryAllocate>();
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                serviceDescriptors
                    .AddSingleton<IMemoryAllocate, WindowsMemoryAllocate>();
            }

            return serviceDescriptors;
        }
    }
}
