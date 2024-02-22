using System.IO.Compression;
using System.Runtime.Versioning;

using PalworldModInstaller.Models;

using Spectre.Console;

namespace PalworldModInstaller.Services {
    [SupportedOSPlatform("windows")]
    public class WindowsInstaller : IInstaller {

        public void UninstallFiles(InstallerOptions installerOptions) {
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

        public void InstallFiles(InstallerOptions installerOptions) {
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            var win64Folder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Win64");

            var dotnetDependenciesFolder = Path.Combine(win64Folder, "ManagedModFramework");
            var clrHostLocation = Path.Combine(win64Folder, "CLRHost.dll");
            var netHostLib = Path.Combine(win64Folder, "nethost.dll");
            var entryPELocation = Path.Combine(win64Folder, "Palworld-Win64-Shipping.exe");
            var renamePELocation = Path.Combine(win64Folder, "Game-Palworld-Win64-Shipping.exe");

            if (installerOptions.CreateModsFolder && !Directory.Exists(modsFolder)) {
                AnsiConsole.WriteLine("Default mod folder did not exist, creating it now. (use -m flag to disable this)");
                Directory.CreateDirectory(modsFolder);
            }

            if (!Directory.Exists(dotnetDependenciesFolder)) {
                AnsiConsole.WriteLine("Framework install folder did not exist, creating it now.");
                Directory.CreateDirectory(dotnetDependenciesFolder);
            }

            if (!installerOptions.CheckUpdates && File.Exists(renamePELocation)) {
                AnsiConsole.WriteLine("Renamed PE was found during install process! Are you sure mod loader is not already installed? If you would like to update run with the -u flag.");
                AnsiConsole.WriteLine("Aborting...");
                return;
            }

            if (installerOptions.CheckUpdates) {
                AnsiConsole.WriteLine("Install located, checking for updates...");
                if (!IsOutOfDate()) {
                    AnsiConsole.WriteLine("the newest release is already installed.");
                    return;
                }
            }


            AnsiConsole.WriteLine("Installing mod loader...");
            File.Move(entryPELocation, renamePELocation);
            InstallNewFiles(dotnetDependenciesFolder, entryPELocation, clrHostLocation);
        }

        private void InstallNewFiles(string dotnetDependenciesFolder, string bootstrapper, string clrHost) {
            Task.Factory.StartNew(async () => await UnzipFrameworkPackage(dotnetDependenciesFolder));

            Task.Factory.StartNew(async () => await WriteGithubFile(clrHost, "CLRHost.dll"));
            Task.Factory.StartNew(async () => await WriteGithubFile(bootstrapper, "Bootstrapper.exe"));
        }

        private bool IsOutOfDate() {
            return true;
        }

        private async Task WriteGithubFile(string localLocation, string githubFilename) {
            var fileBytes = await Program.DownloadGithubRelease(githubFilename);
            await File.WriteAllBytesAsync(localLocation, fileBytes);
        }

        private async Task UnzipFrameworkPackage(string extractPath) {
            var githubFileName = "ManagedModFramework.zip";
            var zipFileBytes = await Program.DownloadGithubRelease(githubFileName);
            await File.WriteAllBytesAsync(githubFileName, zipFileBytes);

            using var archive = ZipFile.OpenRead(githubFileName);
            foreach (var entry in archive.Entries) {
                var destinationPath = Path.Combine(extractPath, entry.FullName);
                entry.ExtractToFile(destinationPath, overwrite: true);
            }
        }
    }
}
