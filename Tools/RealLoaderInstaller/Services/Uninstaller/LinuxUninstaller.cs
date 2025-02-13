using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services.Interfaces;

using Spectre.Console;

namespace RealLoaderInstaller.Services.Uninstaller {
    public class LinuxUninstaller : IUninstaller {
        private readonly IModBackupService _modBackupService;

        public LinuxUninstaller(IModBackupService modBackupService) {
            _modBackupService = modBackupService;
        }

        public async Task UninstallFiles(InstallerOptions installerOptions) {
            AnsiConsole.WriteLine($"Removing Mods Folder...");
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            // Check if backup requested.
            _modBackupService.BackupMods(modsFolder);
            Directory.Delete(modsFolder, true);

            AnsiConsole.WriteLine($"Removing Dependencies Folder...");
            var dotnetDependenciesFolder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Linux", "RealLoaderFramework");
            Directory.Delete(dotnetDependenciesFolder, true);

            AnsiConsole.WriteLine($"Restoring launchs script...");
            var launchScript = Path.Combine(installerOptions.InstallLocation, "PalServer.sh");
            RestoreLaunchScript(launchScript);

            AnsiConsole.WriteLine($"Modloader Uninstalled!");
        }

        private void RestoreLaunchScript(string launchScript) {
            File.WriteAllText(launchScript, @$"#!/bin/sh
UE_TRUE_SCRIPT_NAME=$(echo \""$0\"" | xargs readlink -f)
UE_PROJECT_ROOT=$(dirname ""$UE_TRUE_SCRIPT_NAME"")
chmod +x ""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test""
""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test"" Pal ""$@"" 
");
        }
    }
}
