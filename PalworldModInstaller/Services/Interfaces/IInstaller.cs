using PalworldModInstaller.Models;

namespace PalworldModInstaller.Services.Interfaces {
    public interface IInstaller {
        Task InstallFiles(InstallerOptions installerOptions);
    }
}