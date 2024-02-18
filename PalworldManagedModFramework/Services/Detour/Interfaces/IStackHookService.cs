using PalworldManagedModFramework.Services.Detour.Models;

namespace PalworldManagedModFramework.Services.Detour.Interfaces {
    public interface IStackHookService {
        void InstallHook(nint hookAddress, nint redirect);
        void UninstallHook(InstalledHook installedHook);
    }
}