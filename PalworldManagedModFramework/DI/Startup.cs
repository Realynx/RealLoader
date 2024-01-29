using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.Models.Config;
using PalworldManagedModFramework.Services;
using PalworldManagedModFramework.Services.Logging;

namespace PalworldManagedModFramework.DI {
    public static class Startup {
        public static IConfigurationRoot Configuration { get; internal set; }

        internal static void Configure(HostBuilderContext context, IConfigurationBuilder configurationBuilder) {
            configurationBuilder
               .AddJsonFile("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld\\Pal\\Binaries\\Win64\\ManagedModFramework\\AppSettings.json", true);
        }

        internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services) {

            services
                .AddSingleton<LoggerConfig>();

            services
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<GameExplorer>();
        }
    }
}