using RealLoaderInstaller.Models;

namespace RealLoaderInstaller.Services.Uninstaller {
    public interface IUninstaller {
        Task UninstallFiles(InstallerOptions installerOptions);
    }
}