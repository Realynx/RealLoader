using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services.Interfaces;

using Spectre.Console;

namespace RealLoaderInstaller.Services.Uninstaller {
    public class WindowsUninstaller : IUninstaller {
        private readonly IModBackupService _modBackupService;

        public WindowsUninstaller(IModBackupService modBackupService) {
            _modBackupService = modBackupService;
        }

        public async Task UninstallFiles(InstallerOptions installerOptions) {
            var win64Folder = GetWin64Folder(installerOptions.InstallLocation);
            var dotnetDependenciesFolder = Path.Combine(win64Folder, "RealLoaderFramework");

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

        private string GetWin64Folder(string rootFolder) {
            var win64directory = Directory.EnumerateDirectories(rootFolder, "*", SearchOption.AllDirectories)
                .SingleOrDefault(i => i.EndsWith(Path.Combine("Binaries", "Win64")));

            return win64directory is null
                ? throw new DirectoryNotFoundException("Could not find Win64 folder.")
                : win64directory;
        }
    }
}
