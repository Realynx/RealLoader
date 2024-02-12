using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.Models.Config;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Sdk.Services.Memory.Windows;

using PalworldManagedModFramework.Services.AssemblyLoading;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning.Linux;
using PalworldManagedModFramework.Services.SandboxDI;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

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
                    .AddSingleton<IMemoryScanner, LinuxMemoryScanner>();
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                services
                    .AddSingleton<IEnginePattern, WindowsClientPattern>()
                    .AddSingleton<IUObjectFuncs, WindowsUObjectFuncs>()
                    .AddSingleton<IProcessSuspender, WindowsProcessSuspender>()
                    .AddSingleton<IMemoryMapper, WindowsMemoryMapper>()
                    .AddSingleton<IMemoryScanner, WindowsMemoryScanner>();
            }

            services
                .AddSingleton<IOperandResolver, OperandResolver>()
                .AddSingleton<IPatternResolver, PatternResolver>()
                .AddSingleton<UReflectionPointerScanner>()
                .AddSingleton<ISequenceScanner, SequenceScanner>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IAssemblyDiscovery, AssemblyDiscovery>()
                .AddSingleton<IModLoader, ModLoader>()
                .AddSingleton<ISandboxDIService, SandboxDIService>()
                .AddSingleton<UnrealReflection>()
                .AddSingleton<IGlobalObjects, GlobalObjects>();
        }
    }
}