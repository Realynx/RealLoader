﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.Models.Config;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services;
using PalworldManagedModFramework.PalWorldSdk.Services.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows;
using PalworldManagedModFramework.Services.AssemblyLoading;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning;
using PalworldManagedModFramework.Services.MemoryScanning.EnginePatterns;

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
                    .AddSingleton<IProcessSuspender, LinuxProcessSuspender>()
                    .AddSingleton<IMemoryMapper, LinuxMemoryMapper>();
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                services
                    .AddSingleton<IEnginePattern, WindowsClientPattern>()
                    .AddSingleton<IProcessSuspender, WindowsProcessSuspender>()
                    .AddSingleton<IMemoryMapper, WindowsMemoryMapper>();
            }

            services
                .AddSingleton<UReflectionPointerScanner>()
                .AddSingleton<ISequenceScanner, SequenceScanner>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IAssemblyDiscovery, AssemblyDiscovery>()
                .AddSingleton<IModLoader, ModLoader>();
        }
    }
}