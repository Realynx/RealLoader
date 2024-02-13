using PalworldManagedModFramework.Sdk.Services;

namespace PalworldManagedModFramework.Services.SandboxDI.ServiceResolution {
    public class SandboxServiceProvider : IServiceProvider, IDisposable {
        private readonly IServiceProvider _rootServiceProvider;
        private readonly IServiceProvider _sandboxServiceProvider;

        public SandboxServiceProvider(IServiceProvider root, IServiceProvider sandbox) {
            _rootServiceProvider = root;
            _sandboxServiceProvider = sandbox;
        }

        public object GetService(Type serviceType) {
            var service = _rootServiceProvider.GetService(serviceType);
            service ??= _sandboxServiceProvider.GetService(serviceType);

            if (service is null) {
                DebugUtilities.WaitForDebuggerAttach();
            }

            return service;
        }

        public void Dispose() {
            if (_rootServiceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}
