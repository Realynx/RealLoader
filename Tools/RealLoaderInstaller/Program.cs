using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Extensions.Http;

using RealLoaderInstaller.Commands;
using RealLoaderInstaller.DI;
using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services;
using RealLoaderInstaller.Services.Installer;
using RealLoaderInstaller.Services.Interfaces;
using RealLoaderInstaller.Services.Uninstaller;

using Spectre.Console.Cli;

namespace RealLoaderInstaller {
    public class Program {
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
                .AddSingleton<InstallerOptions>()
                .AddSingleton<IModBackupService, ModBackupService>()
                .AddSingleton<IModFileService, ModFileService>()
                .AddSingleton<IGithubArtifactDownloader, GithubArtifactDownloader>()
                .AddSingleton<IGithubArchiveManager, GithubArchiveManager>();

            serviceDescriptors
                .AddHttpClient<IGithubArtifactDownloader, GithubArtifactDownloader>(client => {
                    client.DefaultRequestHeaders.UserAgent.Clear();
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("RealLoader_Installer/1.0 (Windows; Linux; https://github.com/Realynx/RealLoader)");
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = true, })
                .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
