using PalworldModInstaller.Models;
using PalworldModInstaller.Services.Interfaces;

using Spectre.Console;

namespace PalworldModInstaller.Services {
    public class ModFileService : IModFileService {
        private readonly IGithubArtifactDownloader _githubArtifactDownloader;
        private readonly InstallerOptions _installerOptions;

        public ModFileService(IGithubArtifactDownloader githubArtifactDownloader, InstallerOptions installerOptions) {
            _githubArtifactDownloader = githubArtifactDownloader;
            _installerOptions = installerOptions;
        }

        public async Task InstallNewFiles(string dotnetDependenciesFolder, string clrHost) {
            AnsiConsole.WriteLine("Downloading Github Artifacts...");

            await _githubArtifactDownloader.UnzipFrameworkPackage(dotnetDependenciesFolder);
            await WriteGithubFile(clrHost, "CLRHost.dll");
        }

        public async Task WriteGithubFile(string localLocation, string githubFilename) {
            var fileBytes = await _githubArtifactDownloader.DownloadGithubReleaseAsync(githubFilename);
            await File.WriteAllBytesAsync(localLocation, fileBytes);
        }

        public void CheckFrameworkInstallFolder(string dotnetDependenciesFolder) {
            if (!Directory.Exists(dotnetDependenciesFolder)) {
                AnsiConsole.WriteLine("Framework install folder did not exist, creating it now.");
                Directory.CreateDirectory(dotnetDependenciesFolder);
            }
        }

        public void CheckClrModsFolder(string modsFolder) {
            if (_installerOptions.CreateModsFolder && !Directory.Exists(modsFolder)) {
                AnsiConsole.WriteLine("Default mod folder did not exist, creating it now. (use -m flag to disable this)");
                Directory.CreateDirectory(modsFolder);
            }
        }
    }
}
