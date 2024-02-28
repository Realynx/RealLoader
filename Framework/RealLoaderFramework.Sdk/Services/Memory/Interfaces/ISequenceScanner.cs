using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface ISequenceScanner {
        nint[][] ScanMemoryRegion(ByteCodePattern[] patterns, MemoryRegion memoryRegion);
        nint[][] ScanMemoryRegions(ByteCodePattern[] patterns, IEnumerable<MemoryRegion> memoryRegions);
    }
}