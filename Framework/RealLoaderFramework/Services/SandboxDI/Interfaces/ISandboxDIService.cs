using RealLoaderFramework.Sdk.Interfaces;

namespace RealLoaderFramework.Services.SandboxDI.Interfaces {
    public interface ISandboxDIService {
        void DestroyProvider(ISbStartup serviceContainerMod);
        void InitServiceProvider(ISbStartup serviceContainerMod);
        object ResolveService(Type serviceType, ISbStartup sbStartupMod = null);
    }
}