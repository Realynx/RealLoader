using PalworldModInstaller.Models;

namespace PalworldModInstaller.Services {
    internal interface Installer {
        void InstallFiles(InstallerOptions installerOptions);
    }
}