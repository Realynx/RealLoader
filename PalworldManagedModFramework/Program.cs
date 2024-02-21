using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.DI;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;

namespace PalworldManagedModFramework {
    internal static class Program {

        internal delegate void VoidDelegateSignature();
        public static void EntryPoint() {
            try {
                Console.OutputEncoding = Encoding.UTF8;

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

                ConsoleExtensions.SetWindowAlwaysOnTop(loggerInstance);

                var modLoader = host.Services.GetRequiredService<IModLoader>();
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