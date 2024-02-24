using System.Runtime.Versioning;

using PalworldModInstaller.Models;
using PalworldModInstaller.Services.Interfaces;

using Spectre.Console;

namespace PalworldModInstaller.Services.Installer {
    [SupportedOSPlatform("linux")]
    public class LinuxInstaller : IInstaller {
        public async Task InstallFiles(InstallerOptions installerOptions) {
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            var dotnetDependenciesFolder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Linux", "ManagedModFramework");

            var launchScript = Path.Combine(installerOptions.InstallLocation, "PalServer.sh");


            if (installerOptions.CreateModsFolder && !Directory.Exists(modsFolder)) {
                AnsiConsole.WriteLine("Default mod folder did not exist, creating it now. (use -m flag to disable this)");
                Directory.CreateDirectory(modsFolder);
            }

            if (!installerOptions.CheckUpdates && Directory.Exists(dotnetDependenciesFolder) && Directory.GetFiles(dotnetDependenciesFolder).Length > 0) {
                AnsiConsole.WriteLine("Framework was found during install process! If you would like to update run with the -u flag.");
                AnsiConsole.WriteLine("Aborting...");
                return;
            }

            if (!Directory.Exists(dotnetDependenciesFolder)) {
                AnsiConsole.WriteLine("Framework install folder did not exist, creating it now.");
                Directory.CreateDirectory(dotnetDependenciesFolder);
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

        private void EditLaunchScript(string launchScript) {
            File.WriteAllText(launchScript, @$"#!/bin/sh
UE_TRUE_SCRIPT_NAME=$(echo \""$0\"" | xargs readlink -f)
UE_PROJECT_ROOT=$(dirname ""$UE_TRUE_SCRIPT_NAME"")
chmod +x ""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test""
""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test"" Pal ""$@"" 

# Run PalServer-Linux-Test with LD_PRELOAD set only for this command
LD_PRELOAD=""$UE_PROJECT_ROOT/Pal/Binaries/Linux/ManagedModFramework/CLRHost.so"" ""$UE_PROJECT_ROOT/Pal/Binaries/Linux/PalServer-Linux-Test"" Pal ""$@""
");
        }
    }
}
