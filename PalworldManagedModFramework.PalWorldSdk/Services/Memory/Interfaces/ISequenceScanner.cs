using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface ISequenceScanner {
        nint[][] ScanMemoryRegion(string[] signatures, MemoryRegion memoryRegion);
        nint[][] ScanMemoryRegions(string[] signatures, IEnumerable<MemoryRegion> memoryRegions);
    }
}