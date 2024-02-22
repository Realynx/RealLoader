using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using PalworldModInstaller.Commands;
using PalworldModInstaller.DI;
using PalworldModInstaller.Services;

using Spectre.Console;
using Spectre.Console.Cli;

namespace PalworldModInstaller {
    internal class Program {
        internal const string LATEST_GITHUB_ADDRESS = "https://github.com/PoofImaFox/PalworldManagedModFramework/releases/latest/download/";

        internal static void Main(string[] args) {
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
                    .AddSingleton<LinuxInstaller>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                serviceDescriptors
                    .AddSingleton<WindowsInstaller>();
            }
        }

        public static async Task<byte[]> DownloadGithubRelease(string githubFileName) {
            var httpClient = new HttpClient();
            var httpFileResponse = await httpClient.GetAsync($"{Program.LATEST_GITHUB_ADDRESS}{githubFileName}");
            if (!httpFileResponse.IsSuccessStatusCode) {
                AnsiConsole.WriteLine($"Failed to download '{githubFileName}'...");
            }

            var httpFileBytes = await httpFileResponse.Content.ReadAsByteArrayAsync();
            return httpFileBytes;
        }
    }
}
