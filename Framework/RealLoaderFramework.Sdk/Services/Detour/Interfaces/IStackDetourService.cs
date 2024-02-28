using RealLoaderFramework.Sdk.Services.Detour.Models;

namespace RealLoaderFramework.Sdk.Services.Detour.Interfaces {
    public interface IStackDetourService {
        DetourRecord PrepareDetour(nint detourAddress, nint redirect);
    }
}