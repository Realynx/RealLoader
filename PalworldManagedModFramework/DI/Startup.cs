using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.Models.Config;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.FunctionServices;
using PalworldManagedModFramework.Sdk.Services;
using PalworldManagedModFramework.Sdk.Services.Detour;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Linux;
using PalworldManagedModFramework.Sdk.Services.Detour.Windows;
using PalworldManagedModFramework.Sdk.Services.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Sdk.Services.Memory.Windows;
using PalworldManagedModFramework.Sdk.Services.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.UnrealHook.Interfaces;
using PalworldManagedModFramework.Services.AssemblyLoading;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning;
using PalworldManagedModFramework.Services.SandboxDI;
using PalworldManagedModFramework.Services.SandboxDI.Interfaces;

namespace PalworldManagedModFramework.DI {
    internal static class Startup {
        internal static IConfigurationRoot Configuration { get; set; }

        internal static void Configure(HostBuilderContext context, IConfigurationBuilder configurationBuilder) {
            configurationBuilder
               .AddJsonFile("AppSettings.json", false);
        }

        internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services) {

            services
                .AddSingleton<LoggerConfig>()
                .AddSingleton<ModLoaderConfig>();

            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                services
                    .AddSingleton<IEnginePattern, LinuxServerPattern>()
                    //.AddSingleton<IUObjectFuncs, LinuxUObjectFuncs>()
                    .AddSingleton<IProcessSuspender, LinuxProcessSuspender>()
                    .AddSingleton<IMemoryMapper, LinuxMemoryMapper>()
                    .AddSingleton<IMemoryScanner, LinuxMemoryScanner>()
                    .AddSingleton<IMemoryAllocate, LinuxMemoryAllocate>();
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                services
                    .AddSingleton<IEnginePattern, WindowsClientPattern>()
                    .AddSingleton<IUObjectFuncs, WindowsUObjectFuncs>()
                    .AddSingleton<IProcessSuspender, WindowsProcessSuspender>()
                    .AddSingleton<IMemoryMapper, WindowsMemoryMapper>()
                    .AddSingleton<IMemoryScanner, WindowsMemoryScanner>()
                    .AddSingleton<IMemoryAllocate, WindowsMemoryAllocate>();
            }


            //To be tested on both OS
            services
                .AddSingleton<IShellCodeFactory, ShellCodeFactory>();

            // TODO: Create extention classes with batches of these add singletons in logical groupings.

            services
                .AddSingleton<ILogger, Logger>()

                .AddSingleton<IOperandResolver, OperandResolver>()
                .AddSingleton<IPropertyManager, PropertyManager>()
                .AddSingleton<IBulkTypePatternScanner, BulkTypePatternScanner>()
                .AddSingleton<PatternScanner>()
                .AddSingleton<ISequenceScanner, SequenceScanner>()
                .AddSingleton<IStackDetourService, StackDetourService>()
                .AddSingleton<IInstructionPatcher, InstructionPatcher>()
                .AddSingleton<IDetourManager, DetourManager>()
                .AddSingleton<IDetourAttributeScanner, DetourAttributeScanner>()
                .AddSingleton<IShellCodeReader, ShellCodeReader>()

                .AddSingleton<IAssemblyDiscovery, AssemblyDiscovery>()
                .AddSingleton<IModLoader, ModLoader>()
                .AddSingleton<ISandboxDIService, SandboxDIService>()
                .AddSingleton<IUnrealReflection, UnrealReflection>()
                .AddSingleton<IGlobalObjects, GlobalObjects>()
                .AddSingleton<INamePoolService, NamePoolService>();

            // TODO: Immediate Loading.
            services
                .AddSingleton<IUnrealHookManager, UnrealHookManager>();
        }
    }
}