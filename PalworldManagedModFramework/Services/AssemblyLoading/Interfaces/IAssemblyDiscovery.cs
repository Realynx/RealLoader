using PalworldManagedModFramework.Models;

namespace PalworldManagedModFramework.Services.AssemblyLoading.Interfaces {
    public interface IAssemblyDiscovery {
        IEnumerable<ClrMod> DiscoverValidModAssembies();
    }
}