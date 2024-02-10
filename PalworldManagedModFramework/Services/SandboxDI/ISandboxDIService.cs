using PalworldManagedModFramework.Sdk.Interfaces;

namespace PalworldManagedModFramework.Services.SandboxDI {
    public interface ISandboxDIService {
        void DestroyProvider(ISbStartup serviceContainerMod);
        void InitServiceProvider(ISbStartup serviceContainerMod);
        object ResolveService(Type serviceType, ISbStartup sbStartupMod = null);
    }
}