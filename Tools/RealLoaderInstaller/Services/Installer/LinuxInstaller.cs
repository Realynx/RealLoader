using System.Runtime.Versioning;

using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services.Interfaces;

using Spectre.Console;

namespace RealLoaderInstaller.Services.Installer {
    [SupportedOSPlatform("linux")]
    public class LinuxInstaller : IInstaller {
        private readonly IGithubArtifactDownloader _githubArtifactDownloader;
        private readonly IModFileService _modFileService;

        public LinuxInstaller(IGithubArtifactDownloader githubArtifactDownloader, IModFileService modFileService) {
            _githubArtifactDownloader = githubArtifactDownloader;
            _modFileService = modFileService;
        }

        public async Task InstallFiles(InstallerOptions installerOptions) {

            AnsiConsole.WriteLine("Checking Mods Folder...");
            var modsFolder = Path.Combine(installerOptions.InstallLocation, "ClrMods");
            _modFileService.CheckClrModsFolder(modsFolder);

            AnsiConsole.WriteLine("Checking Dependancies Folder...");
            var dotnetDependenciesFolder = Path.Combine(installerOptions.InstallLocation, "Pal", "Binaries", "Linux", "ManagedModFramework");
            _modFileService.CheckFrameworkInstallFolder(dotnetDependenciesFolder);


            if (!installerOptions.CheckUpdates) {
                return;
            }

            AnsiConsole.WriteLine("Checking for updates...");
            var realLoaderFramework = Path.Combine(dotnetDependenciesFolder, "RealLoaderFramework.dll");
            if (!await _githubArtifactDownloader.IsOutOfDate(realLoaderFramework)) {
                AnsiConsole.WriteLine("the newest release is already installed.");
                return;
            }

            AnsiConsole.WriteLine("Installing new files...");
            var clrHostLocation = Path.Combine(dotnetDependenciesFolder, "CLRHost.dll");
            await _modFileService.InstallNewFiles(dotnetDependenciesFolder, clrHostLocation);

            AnsiConsole.WriteLine("Editing server launch script...");
            var launchScript = Path.Combine(installerOptions.InstallLocation, "PalServer.sh");
            EditLaunchScript(launchScript);
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
