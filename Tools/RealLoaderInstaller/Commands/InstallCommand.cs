using System.Diagnostics.CodeAnalysis;

using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services.Interfaces;
using RealLoaderInstaller.Services.Uninstaller;

using Spectre.Console.Cli;

namespace RealLoaderInstaller.Commands {
    public class InstallCommand : Command<InstallerOptions> {
        private readonly IInstaller _installer;
        private readonly IUninstaller _uninstaller;
        private readonly InstallerOptions _installerOptions;
        private InstallerOptions _settings;

        public InstallCommand(IInstaller installer, IUninstaller uninstaller, InstallerOptions installerOptions) {
            _installer = installer;
            _uninstaller = uninstaller;
            _installerOptions = installerOptions;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] InstallerOptions settings) {
            _settings = settings;

            var installerTask = Task.Factory.StartNew(ExecuteAsync);
            var success = installerTask.Wait(TimeSpan.FromSeconds(40));

            UpdatePropertyValues(settings);
            return success ? 0 : -1;
        }

        private void UpdatePropertyValues(InstallerOptions settings) {
            var propeties = _installerOptions.GetType().GetProperties();
            foreach (var property in propeties) {
                var configuredValue = property.GetValue(settings);
                property.SetValue(_installerOptions, configuredValue);
            }
        }

        private void ExecuteAsync() {
            if (_settings.Uninstall) {
                _uninstaller.UninstallFiles(_settings);
            }
            else {
                _installer.InstallFiles(_settings);
            }
        }
    }
}
