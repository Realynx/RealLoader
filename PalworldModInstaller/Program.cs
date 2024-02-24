using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using PalworldModInstaller.Commands;
using PalworldModInstaller.DI;
using PalworldModInstaller.Services.Installer;
using PalworldModInstaller.Services.Interfaces;
using PalworldModInstaller.Services.Uninstaller;

using Polly.Extensions.Http;
using Polly;

using Spectre.Console.Cli;
using PalworldModInstaller.Services;

namespace PalworldModInstaller {
    public class Program {
        private const string LATEST_GITHUB_ADDRESS = "https://github.com/PowerShell/PowerShell/releases/latest/download/";

        public static void Main(string[] args) {
            var registrar = ConfigureServiceRegistry();

            var app = new CommandApp<InstallCommand>(registrar);
            app.Run(args);
        }

        private static TypeRegistrar ConfigureServiceRegistry() {
            var registrations = new ServiceCollection();
            ConfigureServices(registrations);

            var registrar = new TypeRegistrar(registrations);
            return registrar;
        }

        private static void ConfigureServices(IServiceCollection serviceDescriptors) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                serviceDescriptors
                    .AddSingleton<IInstaller, LinuxInstaller>()
                    .AddSingleton<IUninstaller, LinuxUninstaller>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                serviceDescriptors
                    .AddSingleton<IInstaller, WindowsInstaller>()
                    .AddSingleton<IUninstaller, WindowsUninstaller>();
            }

            serviceDescriptors
                .AddSingleton<IGithubArtifactDownloader, GithubArtifactDownloader>();

            serviceDescriptors
                .AddHttpClient<IGithubArtifactDownloader, GithubArtifactDownloader>(client => {
                    client.BaseAddress = new Uri(LATEST_GITHUB_ADDRESS);
                    client.DefaultRequestHeaders.UserAgent.Clear();
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("ManagedModWorks_Installer/1.0 (Windows; Linux; https://github.com/PoofImaFox/PalworldManagedModFramework)");
                })
                .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
