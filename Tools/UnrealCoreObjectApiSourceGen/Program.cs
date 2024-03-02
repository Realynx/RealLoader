using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using UnrealCoreObjectApiSourceGen.DI;
using UnrealCoreObjectApiSourceGen.Services.SourceGen;

namespace UnrealCoreObjectApiSourceGen {
    internal class Program {
        static void Main(string[] args) {

            var originalDir = Environment.CurrentDirectory;
            var hostBuilder = new HostBuilder()
                .UseConsoleLifetime();

            // Call startup functions to configure DI Container.
            hostBuilder.ConfigureAppConfiguration(Startup.Configure);
            hostBuilder.ConfigureAppConfiguration((_, config) => Startup.Configuration = config.Build());
            hostBuilder.ConfigureServices(Startup.ConfigureServices);

            var host = hostBuilder
                .UseSerilog()
                .Build();

            var codeParser = host.Services.GetRequiredService<INativeCodeParser>();

            Environment.CurrentDirectory = "C:\\Users\\poofi\\source\\repos\\UnrealEngine\\Engine";
            var parsed = codeParser.ParseSourceFile("Source\\Runtime\\CoreUObject\\Public\\UObject\\Class.h", out var libClangTranslationRecord);
            Console.WriteLine($"Parsed: {parsed}");

            if (!parsed || libClangTranslationRecord is null) {
                return;
            }

            var languageConverter = host.Services.GetRequiredService<ILanguageConverter>();

            var sdk = Path.Combine(originalDir, "CSharpCoreUObjectSDK");
            languageConverter.ConvertLanugague(sdk, libClangTranslationRecord!, ConversionLanguage.CSharp);

            libClangTranslationRecord!.Dispose();

            for (; ; )
                Console.ReadLine();
        }
    }
}
