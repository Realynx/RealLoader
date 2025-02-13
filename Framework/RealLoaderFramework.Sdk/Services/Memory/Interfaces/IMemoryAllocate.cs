using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface IMemoryAllocate {
        nint Allocate(SimpleMemoryProtection protection, uint length);
        bool Free(nint address);
        bool SetProtection(nint address, uint length, SimpleMemoryProtection protection, out SimpleMemoryProtection previousProtection);
    }
}