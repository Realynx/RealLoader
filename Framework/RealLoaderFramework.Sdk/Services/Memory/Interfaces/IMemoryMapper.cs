using System.Diagnostics;

using RealLoaderFramework.Sdk.Services.Memory.Models;

using static RealLoaderFramework.Sdk.Services.Memory.Linux.NativeFunctions;

namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface IMemoryMapper {
        MemoryRegion[] FindMemoryRegions();
        nint GetBaseAddress();
    }
}