using PalworldModInstaller.Models;
using PalworldModInstaller.Services.Interfaces;

using Spectre.Console;

namespace PalworldModInstaller.Services {
    public class ModBackupService : IModBackupService {
        private readonly InstallerOptions _installerOptions;

        public ModBackupService(InstallerOptions installerOptions) {
            _installerOptions = installerOptions;
        }

        public void BackupMods(string modsFolder) {
            if (!string.IsNullOrWhiteSpace(_installerOptions.Backup)) {
                AnsiConsole.WriteLine($"Backing up mods...");

                if (!Directory.Exists(_installerOptions.Backup)) {
                    AnsiConsole.WriteLine("Backup directory did not exist creating it now...");
                    Directory.CreateDirectory(_installerOptions.Backup);
                }

                var mods = Directory.GetDirectories(modsFolder);
                var backedup = 0;
                foreach (var mod in mods) {
                    var nwPath = Path.Combine(_installerOptions.Backup, Path.GetFileName(mod));

                    File.Move(mod, nwPath);
                    backedup++;
                }

                AnsiConsole.WriteLine($"Backed up {backedup} mods.");
            }
        }
    }
}
