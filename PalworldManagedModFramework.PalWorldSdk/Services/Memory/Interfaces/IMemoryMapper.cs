using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IMemoryMapper {
        MemoryRegion[] FindMemoryRegions();
        nint GetBaseAddress();
    }
}