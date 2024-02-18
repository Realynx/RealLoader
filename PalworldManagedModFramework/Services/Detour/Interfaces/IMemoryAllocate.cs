using PalworldManagedModFramework.Services.Detour.Models;

namespace PalworldManagedModFramework.Services.Detour.Interfaces {
    public interface IMemoryAllocate {
        nint Allocate(MemoryProtection protection, uint length);
        bool Free(nint address, nuint length);
    }
}