using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using UnrealCoreObjectApiSourceGen.DI;
using UnrealCoreObjectApiSourceGen.Services.SourceGen;

namespace UnrealCoreObjectApiSourceGen {
    internal class Program {
        static void Main(string[] args) {

            var hostBuilder = new HostBuilder()
                .UseConsoleLifetime();

            // Call startup functions to configure DI Container.
            hostBuilder.ConfigureAppConfiguration(Startup.Configure);
            hostBuilder.ConfigureAppConfiguration((_, config) => Startup.Configuration = config.Build());
            hostBuilder.ConfigureServices(Startup.ConfigureServices);

            var host = hostBuilder
                .UseSerilog()
                .Build();

            var codeParser = host.Services.GetRequiredService<NativeCodeParser>();
            var parsed = codeParser.ParseSourceFile("C:\\Users\\poofi\\source\\repos\\PalworldManagedModFramework\\UnrealCoreObjectApiSourceGen\\LocalHeaderFiles\\UObject\\Class.h");
            Console.WriteLine($"Parsed: {parsed}");

            for (; ; )
                Console.ReadLine();
        }
    }
}
