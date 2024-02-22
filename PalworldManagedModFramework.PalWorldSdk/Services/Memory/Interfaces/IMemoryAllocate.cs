using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IMemoryAllocate {
        nint Allocate(SimpleMemoryProtection protection, uint length);
        bool Free(nint address);
        bool SetProtection(nint address, uint length, SimpleMemoryProtection protection, out SimpleMemoryProtection previousProtection);
    }
}