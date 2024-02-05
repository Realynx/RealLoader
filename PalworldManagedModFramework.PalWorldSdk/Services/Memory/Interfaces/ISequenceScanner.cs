using System.Diagnostics;

using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Models;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces {
    public interface ISequenceScanner {
        nint[] ScanMemoryRegion(string signature, MemoryRegion memoryRegion);
        IEnumerable<nint> ScanMemoryRegions(string signature, IEnumerable<MemoryRegion> memoryRegions);
    }
}