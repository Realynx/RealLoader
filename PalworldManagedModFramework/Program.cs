using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.DI;
using PalworldManagedModFramework.Services;
using PalworldManagedModFramework.Services.AssemblyLoading;
using PalworldManagedModFramework.Services.Logging;

namespace PalworldManagedModFramework {
    public static class Program {
        public delegate void VoidDelegateSignature();
        public static void EntryPoint() {
            try {
                AppDomainMonitor.MonitorDomain();

                Console.WriteLine($"Loading .NET DI Service Container...");

                var hostBuilder = new HostBuilder()
                    .UseConsoleLifetime();

                // Call startup functions to configure DI Container.
                hostBuilder.ConfigureAppConfiguration(Startup.Configure);
                hostBuilder.ConfigureAppConfiguration((_, config) => Startup.Configuration = config.Build());
                hostBuilder.ConfigureServices(Startup.ConfigureServices);

                var host = hostBuilder
                    .Build();

                var loggerInstance = host.
                    Services.GetRequiredService<ILogger>();

                loggerInstance.Info("DI Container Setup!");

                var modLoader = host.Services.GetRequiredService<ModLoader>();
                modLoader.LoadMods();

                for (; ; )
                    Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
