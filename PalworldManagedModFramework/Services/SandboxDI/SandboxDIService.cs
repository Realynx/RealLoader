using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Services.SandboxDI.Interfaces;
using PalworldManagedModFramework.Services.SandboxDI.ServiceResolution;

namespace PalworldManagedModFramework.Services.SandboxDI {
    public class SandboxDIService : ISandboxDIService {
        private readonly ILogger _logger;
        private readonly IServiceProvider _rootServiceProvider;
        private readonly Dictionary<ISbStartup, IHost> _modContainers = new();

        public SandboxDIService(ILogger logger, IServiceProvider serviceProvider) {
            _logger = logger;
            _rootServiceProvider = serviceProvider;
        }

        public void InitServiceProvider(ISbStartup serviceContainerMod) {
            if (_modContainers.ContainsKey(serviceContainerMod)) {
                _logger.Error($"Mods can only consume a single {nameof(IHost)} instance.");
                return;
            }

            var hostBuilder = new HostBuilder();

            hostBuilder.UseServiceProviderFactory(new SandboxServiceProviderFactory(_rootServiceProvider));

            hostBuilder.ConfigureAppConfiguration(serviceContainerMod.Configure);
            hostBuilder.ConfigureAppConfiguration((_, config) => serviceContainerMod.Configuration = config.Build());

            // This must be set before services get configured, in-case dev needs their config early.
            serviceContainerMod.Configuration = serviceContainerMod.Configuration;
            hostBuilder.ConfigureServices(serviceContainerMod.ConfigureServices);

            var host = hostBuilder.Build();
            _modContainers.Add(serviceContainerMod, host);
            _logger.Debug("Created mod host instance.");
        }

        public void DestroyProvider(ISbStartup serviceContainerMod) {
            if (_modContainers.ContainsKey(serviceContainerMod)) {
                _logger.Error($"Tried to destory an {nameof(IHost)} instance that did not exist anymore.");
                return;
            }

            _modContainers[serviceContainerMod].Dispose();
            _modContainers.Remove(serviceContainerMod);
            _logger.Debug("Cleaned up mod host instance.");
        }

        // TODO: Fix this, un-needed
        public object ResolveService(Type serviceType, ISbStartup sbStartupMod = null) {
            object resolvedService = null;

            if (sbStartupMod is not null && _modContainers.ContainsKey(sbStartupMod)) {
                resolvedService = _modContainers[sbStartupMod].Services.GetService(serviceType);
            }

            if (resolvedService is null) {
                resolvedService = _rootServiceProvider.GetService(serviceType);
            }

            return resolvedService;
        }
    }
}
