using PalworldModInstaller.Models;
using PalworldModInstaller.Services.Interfaces;

using Spectre.Console;

namespace PalworldModInstaller.Services.Uninstaller {
    public class WindowsUninstaller : IUninstaller {
        private readonly IModBackupService _modBackupService;

        public WindowsUninstaller(IModBackupService modBackupService) {
            _modBackupService = modBackupService;
        }

        public async Task UninstallFiles(InstallerOptions installerOptions) {
            var win64Folder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Win64");
            var dotnetDependenciesFolder = Path.Combine(win64Folder, "ManagedModFramework");

            AnsiConsole.WriteLine($"Removing nethost.dll...");
            var netHostLib = Path.Combine(win64Folder, "nethost.dll");
            File.Delete(netHostLib);

            AnsiConsole.WriteLine($"Removing Proxy dll...");
            var proxtyDll = Path.Combine(win64Folder, installerOptions.ProxyDll);
            File.Delete(proxtyDll);

            AnsiConsole.WriteLine($"Removing Mods Folder...");
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            // Check if backup requested.
            _modBackupService.BackupMods(modsFolder);
            Directory.Delete(modsFolder, true);

            AnsiConsole.WriteLine($"Removing Dependencies Folder...");
            Directory.Delete(dotnetDependenciesFolder, true);

            AnsiConsole.WriteLine($"Modloader Uninstalled!");
        }
    }
}
