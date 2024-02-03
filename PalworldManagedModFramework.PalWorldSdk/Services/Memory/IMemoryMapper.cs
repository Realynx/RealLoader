using System.Diagnostics;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    public interface IMemoryMapper {
        MemoryRegion[] FindMemoryRegions(ProcessModule processModule);
    }
}