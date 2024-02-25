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
        private readonly IModFileService _modFileService;

        public WindowsInstaller(IGithubArtifactDownloader githubArtifactDownloader, IModFileService modFileService) {
            _githubArtifactDownloader = githubArtifactDownloader;
            _modFileService = modFileService;
        }

        public async Task InstallFiles(InstallerOptions installerOptions) {
            AnsiConsole.WriteLine("Checking Mods Folder...");
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            _modFileService.CheckClrModsFolder(modsFolder);

            AnsiConsole.WriteLine("Checking Dependancies Folder...");
            var win64Folder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Win64");
            var dotnetDependenciesFolder = Path.Combine(win64Folder, "ManagedModFramework");
            _modFileService.CheckFrameworkInstallFolder(dotnetDependenciesFolder);

            if (!installerOptions.CheckUpdates) {
                return;
            }

            AnsiConsole.WriteLine("Checking for updates...");
            var palworldManagedModFramework = Path.Combine(dotnetDependenciesFolder, "PalworldManagedModFramework.dll");
            if (!await _githubArtifactDownloader.IsOutOfDate(palworldManagedModFramework)) {
                AnsiConsole.WriteLine("the newest release is already installed.");
                return;
            }

            AnsiConsole.WriteLine("Checking for new nethost.dll...");
            CopyNethost(win64Folder);

            AnsiConsole.WriteLine("Installing new files...");
            var clrHostLocation = Path.Combine(dotnetDependenciesFolder, "CLRHost.dll");
            await _modFileService.InstallNewFiles(dotnetDependenciesFolder, clrHostLocation);

            AnsiConsole.WriteLine("Installing proxy dll...");
            var proxyDllLocation = Path.Combine(win64Folder, installerOptions.ProxyDll);
            await _modFileService.WriteGithubFile(proxyDllLocation, installerOptions.ProxyDll);
        }

        private void CopyNethost(string win64Folder) {
            var netHostLib = Path.Combine(win64Folder, "nethost.dll");
            var localNetHost = Path.Combine(FindNewestNetPackPath(), "runtimes", "win-x64", "native", "nethost.dll");
            File.Copy(localNetHost, netHostLib, true);
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
    }
}
