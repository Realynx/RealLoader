using RealLoaderInstaller.Models;

namespace RealLoaderInstaller.Services.Interfaces {
    public interface IInstaller {
        Task InstallFiles(InstallerOptions installerOptions);
    }
}