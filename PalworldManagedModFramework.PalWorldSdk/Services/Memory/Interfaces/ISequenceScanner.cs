using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface ISequenceScanner {
        nint[] ScanMemoryRegion(string signature, MemoryRegion memoryRegion);
        IEnumerable<nint> ScanMemoryRegions(string signature, IEnumerable<MemoryRegion> memoryRegions);
    }
}