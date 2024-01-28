using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.DI;
using PalworldManagedModFramework.Services;
using PalworldManagedModFramework.Services.Logging;

namespace PalworldManagedModFramework {
    public static class Program {
        public static void Main(string[] args) {
            Console.WriteLine($"Loading .NET DI Service Container...");
            Thread.Sleep(TimeSpan.FromSeconds(4));

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

            gameExplorer.Entry();

            Console.ReadLine();
        }
    }
}
