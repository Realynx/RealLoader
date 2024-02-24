using PalworldModInstaller.Models;

namespace PalworldModInstaller.Services.Uninstaller {
    public interface IUninstaller {
        Task UninstallFiles(InstallerOptions installerOptions);
    }
}