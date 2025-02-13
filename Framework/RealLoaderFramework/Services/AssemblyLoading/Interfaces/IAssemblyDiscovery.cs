using RealLoaderFramework.Models;

namespace RealLoaderFramework.Services.AssemblyLoading.Interfaces {
    public interface IAssemblyDiscovery {
        IEnumerable<ClrMod> DiscoverValidModAsselblies();
    }
}