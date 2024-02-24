using System.Diagnostics.CodeAnalysis;

using PalworldModInstaller.Models;
using PalworldModInstaller.Services.Interfaces;
using PalworldModInstaller.Services.Uninstaller;

using Spectre.Console.Cli;

namespace PalworldModInstaller.Commands {
    public class InstallCommand : Command<InstallerOptions> {
        private readonly IInstaller _installer;
        private readonly IUninstaller _uninstaller;
        private InstallerOptions _settings;

        public InstallCommand(IInstaller installer, IUninstaller uninstaller) {
            _installer = installer;
            _uninstaller = uninstaller;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] InstallerOptions settings) {
            _settings = settings;

            var installerTask = Task.Factory.StartNew(ExecuteAsync);
            var success = installerTask.Wait(TimeSpan.FromSeconds(40));

            return success ? 0 : -1;
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
