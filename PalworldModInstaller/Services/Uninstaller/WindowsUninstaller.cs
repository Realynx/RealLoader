using PalworldModInstaller.Models;

using Spectre.Console;

namespace PalworldModInstaller.Services.Uninstaller {
    public class WindowsUninstaller : IUninstaller {
        public WindowsUninstaller() {

        }

        public async Task UninstallFiles(InstallerOptions installerOptions) {
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            var win64Folder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Win64");

            var dotnetDependenciesFolder = Path.Combine(win64Folder, "ManagedModFramework");
            var clrHostLocation = Path.Combine(win64Folder, "CLRHost.dll");
            var entryPELocation = Path.Combine(win64Folder, "Palworld-Win64-Shipping.exe");
            var renamePELocation = Path.Combine(win64Folder, "Game-Palworld-Win64-Shipping.exe");

            var netHostLib = Path.Combine(win64Folder, "nethost.dll");


            if (!string.IsNullOrWhiteSpace(installerOptions.Backup) && !Directory.Exists(installerOptions.Backup)) {
                AnsiConsole.WriteLine("Backup directory did not exist creating it now...");
                Directory.CreateDirectory(installerOptions.Backup);
            }

            if (!string.IsNullOrWhiteSpace(installerOptions.Backup)) {
                var mods = Directory.GetDirectories(modsFolder);

                var modsBackedup = 0;
                foreach (var mod in mods) {
                    var nwPath = Path.Combine(installerOptions.Backup, Path.GetFileName(mod));
                    File.Move(mod, nwPath);
                    modsBackedup++;
                }

                AnsiConsole.WriteLine($"Backed up {modsBackedup} mods.");
            }

            AnsiConsole.WriteLine($"Uninstalling modloader...");

            Directory.Delete(modsFolder, true);
            Directory.Delete(dotnetDependenciesFolder, true);

            File.Delete(netHostLib);
            File.Delete(entryPELocation);
            File.Move(renamePELocation, entryPELocation);

            AnsiConsole.WriteLine($"Modloader uninstalled!");
        }
    }
}
