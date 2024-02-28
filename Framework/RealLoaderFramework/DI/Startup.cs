using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RealLoaderFramework.Models.Config;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Detour;
using RealLoaderFramework.Sdk.Services.Detour.AssemblerServices;
using RealLoaderFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;
using RealLoaderFramework.Sdk.Services.EngineServices;
using RealLoaderFramework.Sdk.Services.Memory;
using RealLoaderFramework.Services;
using RealLoaderFramework.Services.AssemblyLoading;
using RealLoaderFramework.Services.Interfaces;
using RealLoaderFramework.Services.SandboxDI;

namespace RealLoaderFramework.DI {
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
        }
    }
}