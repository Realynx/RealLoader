using Microsoft.Extensions.DependencyInjection;

using PalworldModInstaller.Commands;
using PalworldModInstaller.DI;
using PalworldModInstaller.Services;

using Spectre.Console.Cli;

namespace PalworldModInstaller {
    internal class Program {
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
            serviceDescriptors
                .AddSingleton<LinuxInstaller>()
                .AddSingleton<WindowsInstaller>();
        }
    }
}
