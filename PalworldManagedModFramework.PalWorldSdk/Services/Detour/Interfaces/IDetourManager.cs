using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Interfaces {
    public interface IDetourManager {
        bool InstallDetour(DetourRecord detourRecord);
        DetourRecord PrepareDetour(ManagedDetourInfo managedDetourInfo, nint pFunction);
        bool UninstallDetour(DetourRecord detourRecord);
    }
}