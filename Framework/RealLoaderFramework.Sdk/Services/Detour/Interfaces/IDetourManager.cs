using RealLoaderFramework.Sdk.Services.Detour.Models;

namespace RealLoaderFramework.Sdk.Services.Detour.Interfaces {
    public interface IDetourManager {
        bool InstallDetour(DetourRecord detourRecord);
        DetourRecord PrepareDetour(ManagedDetourInfo managedDetourInfo, nint pFunction);
        bool UninstallDetour(DetourRecord detourRecord);
    }
}