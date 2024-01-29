using System.Diagnostics;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework;
using PalworldManagedModFramework.DI;
using PalworldManagedModFramework.Services;
using PalworldManagedModFramework.Services.Logging;

namespace PalworldManagedModFramework {
    public static class Program {
        public delegate void VoidDelegateSignature();
        public static void EntryPoint() {
            try {
                AppDomainMonitor.MonitorDomain();

                while (!Debugger.IsAttached) {
                    Thread.Sleep(100);
                }

                Debugger.Break();

                var assembly = Assembly.GetExecutingAssembly();
                var clrDirectory = Path.GetDirectoryName(assembly.Location);
                Console.WriteLine(clrDirectory);

                var baseDir = AppContext.BaseDirectory;
                Console.WriteLine(AppContext.BaseDirectory);
                Console.WriteLine($"Loading .NET DI Service Container...");

                Environment.CurrentDirectory = clrDirectory;

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

                var gameExplorer = host.
                    Services.GetRequiredService<GameExplorer>();

                //gameExplorer.Entry();

                Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
