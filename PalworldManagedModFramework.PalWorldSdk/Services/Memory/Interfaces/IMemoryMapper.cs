using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

using static PalworldManagedModFramework.Sdk.Services.Memory.Linux.NativeFunctions;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IMemoryMapper {
        MemoryRegion[] FindMemoryRegions();
        nint GetBaseAddress();
    }
}