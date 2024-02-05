using System.Diagnostics;

using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Models;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces {
    public interface IMemoryMapper {
        MemoryRegion[] FindMemoryRegions();
    }
}