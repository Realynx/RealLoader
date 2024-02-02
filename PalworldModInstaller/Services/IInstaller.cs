using PalworldModInstaller.Models;

namespace PalworldModInstaller.Services {
    public interface IInstaller {
        void InstallFiles(InstallerOptions installerOptions);
        void UninstallFiles(InstallerOptions installerOptions);
    }
}