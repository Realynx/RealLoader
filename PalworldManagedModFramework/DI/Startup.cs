﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.Models.Config;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.EngineServices;
using PalworldManagedModFramework.Sdk.Services.Memory;
using PalworldManagedModFramework.Sdk.Services.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.UnrealHook.Interfaces;
using PalworldManagedModFramework.Services;
using PalworldManagedModFramework.Services.AssemblyLoading;
using PalworldManagedModFramework.Services.Interfaces;
using PalworldManagedModFramework.Services.SandboxDI;

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

            //To be tested on both OS
            services
                .AddSingleton<IShellCodeFactory, ShellCodeFactory>();

            services
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IRuntimeInstaller, RuntimeInstaller>()

                .SetupMemoryScanningServices()
                .SetupBasicUnrealEngineServices()
                .SetupSandboxedDIServices()
                .SetupReflectionModLoader()
                .SetupDetourServices();


            // TODO: Immediate Loading.
            services
                .AddSingleton<IUnrealHookManager, UnrealHookManager>();
        }
    }
}