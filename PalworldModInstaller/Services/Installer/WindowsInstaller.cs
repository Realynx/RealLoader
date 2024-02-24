using System.Runtime.Versioning;

using PalworldModInstaller.Models;
using PalworldModInstaller.Services.Installer.Exceptions;
using PalworldModInstaller.Services.Interfaces;

using Spectre.Console;

namespace PalworldModInstaller.Services.Installer {
    [SupportedOSPlatform("windows")]
    public class WindowsInstaller : IInstaller {
        private const string DOTNET_LOCAL_PACKS = "C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Host.win-x64";
        private readonly IGithubArtifactDownloader _githubArtifactDownloader;

        public WindowsInstaller(IGithubArtifactDownloader githubArtifactDownloader) {
            _githubArtifactDownloader = githubArtifactDownloader;
        }

        public async Task InstallFiles(InstallerOptions installerOptions) {
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            var win64Folder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Win64");

            var dotnetDependenciesFolder = Path.Combine(win64Folder, "ManagedModFramework");
            CopyNethost(win64Folder);

            var clrHostLocation = Path.Combine(dotnetDependenciesFolder, "CLRHost.dll");

            if (installerOptions.CreateModsFolder && !Directory.Exists(modsFolder)) {
                AnsiConsole.WriteLine("Default mod folder did not exist, creating it now. (use -m flag to disable this)");
                Directory.CreateDirectory(modsFolder);
            }

            if (!Directory.Exists(dotnetDependenciesFolder)) {
                AnsiConsole.WriteLine("Framework install folder did not exist, creating it now.");
                Directory.CreateDirectory(dotnetDependenciesFolder);
            }

            if (installerOptions.CheckUpdates) {
                AnsiConsole.WriteLine("Checking for updates...");
                if (!IsOutOfDate()) {
                    AnsiConsole.WriteLine("the newest release is already installed.");
                    return;
                }
            }

            await InstallNewFiles(dotnetDependenciesFolder, clrHostLocation);
        }

        private void CopyNethost(string win64Folder) {
            var netHostLib = Path.Combine(win64Folder, "nethost.dll");
            var localNetHost = Path.Combine(FindNewestNetPackPath(), "runtimes", "win-x64", "native", "nethost.dll");
            File.Copy(localNetHost, netHostLib);
        }

        private async Task InstallNewFiles(string dotnetDependenciesFolder, string clrHost) {
            AnsiConsole.WriteLine("Downloading Github Artifacts...");

            await _githubArtifactDownloader.UnzipFrameworkPackage(dotnetDependenciesFolder);
            await WriteGithubFile(clrHost, "CLRHost.dll");
        }

        private string FindNewestNetPackPath() {
            if (!Directory.Exists(DOTNET_LOCAL_PACKS)) {
                throw new DotnetNotInstalledException("Dotnet was not found on this platform! Please download the latest LTS version from here: https://dotnet.microsoft.com/en-us/download/dotnet");
            }

            var newestSemiVersion = Directory
                .GetDirectories(DOTNET_LOCAL_PACKS, "*.*.*")
                .Select(i => i.Substring(i.LastIndexOf("\\") + 1))
                .Select(Version.Parse)
                .Max();

            return $"{DOTNET_LOCAL_PACKS}\\{newestSemiVersion}";
        }

        private async Task WriteGithubFile(string localLocation, string githubFilename) {
            var fileBytes = await _githubArtifactDownloader.DownloadGithubReleaseAsync(githubFilename);
            await File.WriteAllBytesAsync(localLocation, fileBytes);
        }

        private bool IsOutOfDate() {

            return true;
        }
    }
}
