using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.Models.Config;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.Services;
using PalworldManagedModFramework.Services.AssemblyLoading;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;

namespace PalworldManagedModFramework.DI {
    public static class Startup {
        public static IConfigurationRoot Configuration { get; internal set; }

        internal static void Configure(HostBuilderContext context, IConfigurationBuilder configurationBuilder) {
            configurationBuilder
               .AddJsonFile("AppSettings.json", false);
        }

        internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services) {

            services
                .AddSingleton<LoggerConfig>()
                .AddSingleton<ModLoaderConfig>();

            services
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IAssemblyDiscovery, AssemblyDiscovery>()
                .AddSingleton<IModLoader, ModLoader>();
        }
    }
}