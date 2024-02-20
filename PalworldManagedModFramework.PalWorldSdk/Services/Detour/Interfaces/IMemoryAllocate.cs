using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Interfaces {
    public interface IMemoryAllocate {
        nint Allocate(MemoryProtection protection, uint length);
        bool Free(nint address);
        bool SetProtection(nint address, uint length, MemoryProtection protection, out MemoryProtection previousProtection);
    }
}