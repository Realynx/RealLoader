using PalworldModInstaller.Models;

using Spectre.Console;

namespace PalworldModInstaller.Services {
    internal class WindowsInstaller : Installer {
        public void UninstallFiles(InstallerOptions installerOptions) {

        }

        public void InstallFiles(InstallerOptions installerOptions) {
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            var dotnetDependanciesFolder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Win64", "ManagedModFramework");

            var entryPELocation = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Win64", "Palworld-Win64-Shipping.exe");
            var renamePELocation = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Win64", "Game-Palworld-Win64-Shipping.exe");

            if (installerOptions.CreateModsFolder && !Directory.Exists(modsFolder)) {
                AnsiConsole.WriteLine("Default mod folder did not exist, creating it now. (use -m flag to disable this)");
                Directory.CreateDirectory(modsFolder);
            }

            if (!Directory.Exists(dotnetDependanciesFolder)) {
                AnsiConsole.WriteLine("Framework install folder did not exist, creating it now.");
                Directory.CreateDirectory(dotnetDependanciesFolder);
            }

            if (!installerOptions.CheckUpdates && File.Exists(renamePELocation)) {
                AnsiConsole.WriteLine("Renamed PE was found during install process! Are you sure mod loader is not already installed? If you would like to update run with the -u flag.");
                AnsiConsole.WriteLine("Aborting...");
                return;
            }

            if (installerOptions.CheckUpdates) {
                AnsiConsole.WriteLine("Install located, checking for updates...");

                // TODO: Ping github deployments for updates.
            }
            else {
                AnsiConsole.WriteLine("Installing mod loader...");
                File.Move(entryPELocation, renamePELocation);

                // TODO: Drop the dll files required here as well.
            }
        }
    }
}
