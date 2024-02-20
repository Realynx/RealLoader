using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Interfaces {
    public interface IStackDetourService {
        DetourRecord PrepareDetour(nint detourAddress, nint redirect);
    }
}