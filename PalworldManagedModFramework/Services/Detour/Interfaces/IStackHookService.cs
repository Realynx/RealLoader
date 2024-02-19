using PalworldManagedModFramework.Services.Detour.Models;

namespace PalworldManagedModFramework.Services.Detour.Interfaces {
    public interface IStackHookService {
        InstalledHook InstallHook(nint hookAddress, nint redirect);
        void UninstallHook(InstalledHook installedHook);
    }
}