using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework;
using PalworldManagedModFramework.DI;
using PalworldManagedModFramework.Services;
using PalworldManagedModFramework.Services.Logging;

public static class UnmanagedInterface {
    public delegate void VoidDelegateSignature();
    public static void UnmanagedEntrypoint() {
        Program.EntryPoint();
    }
}

namespace PalworldManagedModFramework {
    public static class Program {
        public static void EntryPoint() {
            File.WriteAllText(@"C:\Program Files (x86)\Steam\steamapps\common\Palworld\Pal\Binaries\Win64\noticeFile.txt", "uwu");

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

            var gameExplorer = host.
                Services.GetRequiredService<GameExplorer>();

            gameExplorer.Entry();

            Console.ReadLine();
        }
    }
}
