using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RealLoaderFramework.DI;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Services.AssemblyLoading.Interfaces;

namespace RealLoaderFramework {
    internal static class Program {
        internal delegate void VoidDelegateSignature();

        public static void EntryPoint() {
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

            var loggerInstance = host.Services.GetRequiredService<ILogger>();
            loggerInstance.Debug("DI Container Setup!");

            ConsoleExtensions.SetWindowAlwaysOnTop(loggerInstance);

            var modLoader = host.Services.GetRequiredService<IModLoader>();
            modLoader.LoadMods();

            loggerInstance.Debug("Mods running, fully loaded.");
            for (;;)
                Console.ReadLine();
        }
    }
}