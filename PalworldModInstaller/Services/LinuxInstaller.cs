using PalworldModInstaller.Models;

using Spectre.Console;

namespace PalworldModInstaller.Services {
    internal class LinuxInstaller : Installer {
        public void UninstallFiles(InstallerOptions installerOptions) {
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            var dotnetDependanciesFolder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Linux", "ManagedModFramework");

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
            Directory.Delete(dotnetDependanciesFolder, true);

            AnsiConsole.WriteLine($"Modloader uninstalled!");
        }

        public void InstallFiles(InstallerOptions installerOptions) {
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            var dotnetDependanciesFolder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Linux", "ManagedModFramework");

            var launchScript = Path.Combine(installerOptions.InstallLocation, "PalServer.sh");


            if (installerOptions.CreateModsFolder && !Directory.Exists(modsFolder)) {
                AnsiConsole.WriteLine("Default mod folder did not exist, creating it now. (use -m flag to disable this)");
                Directory.CreateDirectory(modsFolder);
            }

            if (!installerOptions.CheckUpdates && Directory.Exists(dotnetDependanciesFolder) && Directory.GetFiles(dotnetDependanciesFolder).Length > 0) {
                AnsiConsole.WriteLine("Framework was found during install process! If you would like to update run with the -u flag.");
                AnsiConsole.WriteLine("Aborting...");
                return;
            }

            if (!Directory.Exists(dotnetDependanciesFolder)) {
                AnsiConsole.WriteLine("Framework install folder did not exist, creating it now.");
                Directory.CreateDirectory(dotnetDependanciesFolder);
            }

            if (installerOptions.CheckUpdates) {
                AnsiConsole.WriteLine("Install located, checking for updates...");

                // TODO: Ping github deployments for updates.
                EditLaunchScript(launchScript);
            }
            else {
                AnsiConsole.WriteLine("Installing mod loader...");

                // TODO: Drop the dll files required here as well.
                EditLaunchScript(launchScript);
            }
        }

        private void RestoreLaunchScript(string launchScript) {
            File.WriteAllText(launchScript, @$"#!/bin/sh
UE_TRUE_SCRIPT_NAME=$(echo \""$0\"" | xargs readlink -f)
UE_PROJECT_ROOT=$(dirname ""$UE_TRUE_SCRIPT_NAME"")
chmod +x ""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test""
""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test"" Pal ""$@"" 
");
        }

        private void EditLaunchScript(string launchScript) {
            File.WriteAllText(launchScript, @$"#!/bin/sh
UE_TRUE_SCRIPT_NAME=$(echo \""$0\"" | xargs readlink -f)
UE_PROJECT_ROOT=$(dirname ""$UE_TRUE_SCRIPT_NAME"")
chmod +x ""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test""
""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test"" Pal ""$@"" 

# Run PalServer-Linux-Test with LD_PRELOAD set only for this command
LD_PRELOAD=""$UE_PROJECT_ROOT/pal/Binaries/Linux/ManagedModFramework/CLRHost.so"" ""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test"" Pal ""$@""
");
        }
    }
}
