using System.Runtime.Versioning;

using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services.Installer.Exceptions;
using RealLoaderInstaller.Services.Interfaces;

using Spectre.Console;

namespace RealLoaderInstaller.Services.Installer {
    [SupportedOSPlatform("windows")]
    public class WindowsInstaller : IInstaller {
        private const string DOTNET_LOCAL_PACKS = @"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Host.win-x64";
        private const string NET_HOST = "nethost.dll";

        private readonly IGithubArtifactDownloader _githubArtifactDownloader;
        private readonly IGithubArchiveManager _githubArchiveManager;
        private readonly IModFileService _modFileService;

        public WindowsInstaller(IGithubArtifactDownloader githubArtifactDownloader,
            IGithubArchiveManager githubArchiveManager, IModFileService modFileService) {
            _githubArtifactDownloader = githubArtifactDownloader;
            _githubArchiveManager = githubArchiveManager;
            _modFileService = modFileService;
        }

        public async Task InstallFiles(InstallerOptions installerOptions) {
            AnsiConsole.WriteLine("Checking Mods Folder...");
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            _modFileService.CheckClrModsFolder(modsFolder);

            AnsiConsole.WriteLine("Checking Dependencies Folder...");
            var win64Folder = GetWin64Folder(installerOptions.InstallLocation);
            var dotnetDependenciesFolder = Path.Combine(win64Folder, "RealLoaderFramework");

            _modFileService.CheckFrameworkInstallFolder(dotnetDependenciesFolder);

            AnsiConsole.WriteLine("Checking for updates...");
            var realLoaderFramework = Path.Combine(dotnetDependenciesFolder, "RealLoaderFramework.dll");

            if (File.Exists(realLoaderFramework) && !await _githubArtifactDownloader.IsOutOfDate(realLoaderFramework)) {
                AnsiConsole.WriteLine("the newest release is already installed.");
                return;
            }

            AnsiConsole.WriteLine($"Checking for {NET_HOST}...");
            CopyNethost(win64Folder);

            AnsiConsole.WriteLine("Installing files...");
            var windowsArtifactsArchive = await _githubArchiveManager.CheckoutGithubArchive(GithubArchiveManager.WINDOWS_ARTIFACTS_ZIP);
            var frameworkArchive = await _githubArchiveManager.CheckoutGithubArchive(GithubArchiveManager.FRAMEWORK_ZIP);

            // Clear the existing files.
            Directory.Delete(dotnetDependenciesFolder, true);

            InstallFrameworkFiles(dotnetDependenciesFolder, frameworkArchive);
            InstallClrHost(dotnetDependenciesFolder, windowsArtifactsArchive);
            InstallProxyDll(installerOptions, win64Folder, windowsArtifactsArchive);

            AnsiConsole.WriteLine("Cleaning up archives...");
            windowsArtifactsArchive.tempDirectory.Delete(true);
            frameworkArchive.tempDirectory.Delete(true);

            AnsiConsole.WriteLine("Install Complete!");
        }

        private static void InstallFrameworkFiles(string dotnetDependenciesFolder, GithubArchiveManager.GithubArchive frameworkArchive) {
            AnsiConsole.WriteLine("Installing RealLoaderFramework files...");
            Directory.Move(Path.Combine(frameworkArchive.tempDirectory.FullName, "Framework"), dotnetDependenciesFolder);
        }

        private static void InstallProxyDll(InstallerOptions installerOptions, string win64Folder,
            GithubArchiveManager.GithubArchive githubArchive) {

            AnsiConsole.WriteLine("Installing proxy dll...");
            var proxyDllLocation = Path.Combine(win64Folder, installerOptions.ProxyDll);
            var proxyDll = githubArchive[Path.Combine("Windows", installerOptions.ProxyDll)];

            File.WriteAllBytes(proxyDllLocation, proxyDll);
        }

        private void InstallClrHost(string dotnetDependenciesFolder, GithubArchiveManager.GithubArchive githubArchive) {
            AnsiConsole.WriteLine("Installing ClrHost.dll...");
            var clrHostLocation = Path.Combine(dotnetDependenciesFolder, "CLRHost.dll");
            var clrHost = githubArchive[Path.Combine("Windows", "CLRHost.dll")];

            File.WriteAllBytes(clrHostLocation, clrHost);
        }

        private string GetWin64Folder(string rootFolder) {
            var win64directory = Directory.EnumerateDirectories(rootFolder, "*", SearchOption.AllDirectories)
                .SingleOrDefault(i => i.EndsWith(Path.Combine("Binaries", "Win64")));

            return win64directory is null
                ? throw new DirectoryNotFoundException("Could not find Win64 folder.")
                : win64directory;
        }

        private void CopyNethost(string win64Folder) {
            var netHostLib = Path.Combine(win64Folder, NET_HOST);
            var localNetHost = Path.Combine(FindNewestNetPackPath(), "runtimes", "win-x64", "native", NET_HOST);
            File.Copy(localNetHost, netHostLib, true);
        }

        private string FindNewestNetPackPath() {
            if (!Directory.Exists(DOTNET_LOCAL_PACKS)) {
                throw new DotnetNotInstalledException("Dotnet was not found on this platform! Please download the latest LTS version from here: https://dotnet.microsoft.com/en-us/download/dotnet");
            }

            var newestSemiVersion = Directory
                .GetDirectories(DOTNET_LOCAL_PACKS, "*.*.*")
                .Select(i => i.Substring(i.LastIndexOf('\\') + 1))
                .Select(Version.Parse)
                .Max();

            return $"{DOTNET_LOCAL_PACKS}\\{newestSemiVersion}";
        }
    }
}
