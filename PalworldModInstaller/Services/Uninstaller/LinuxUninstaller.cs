using PalworldModInstaller.Models;

using Spectre.Console;

namespace PalworldModInstaller.Services.Uninstaller {
    public class LinuxUninstaller : IUninstaller {
        public async Task UninstallFiles(InstallerOptions installerOptions) {
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            var dotnetDependenciesFolder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Linux", "ManagedModFramework");

            var launchScript = Path.Combine(installerOptions.InstallLocation, "PalServer.sh");


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
            RestoreLaunchScript(launchScript);
            Directory.Delete(dotnetDependenciesFolder, true);

            AnsiConsole.WriteLine($"Modloader uninstalled!");
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
