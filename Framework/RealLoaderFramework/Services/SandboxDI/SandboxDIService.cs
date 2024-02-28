using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RealLoaderFramework.Sdk.Interfaces;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Services.SandboxDI.Hosting;
using RealLoaderFramework.Services.SandboxDI.Interfaces;
using RealLoaderFramework.Services.SandboxDI.ServiceResolution;

namespace RealLoaderFramework.Services.SandboxDI {
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

            var hostBuilder = new SandboxHostBuilder();
            hostBuilder.UseServiceProviderFactory(new SandboxServiceProviderFactory(_rootServiceProvider));

            var assembly = serviceContainerMod.GetType().Assembly;
            var assemblyDir = Path.GetDirectoryName(assembly.Location);

            var configuration = _rootServiceProvider.GetRequiredService<IConfiguration>();
            hostBuilder.ConfigureAppConfiguration(i => i.AddConfiguration(configuration));

            hostBuilder.ConfigureAppConfiguration(i => serviceContainerMod.Configure(assemblyDir, i));

            // This must be set before services get configured, in-case dev needs their config early.
            hostBuilder.ConfigureAppConfiguration((_, config) => serviceContainerMod.Configuration = config.Build());
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

        public object ResolveService(Type serviceType, ISbStartup sbStartupMod = null) {
            object resolvedService = null;

            if (sbStartupMod is not null && _modContainers.ContainsKey(sbStartupMod)) {
                resolvedService = _modContainers[sbStartupMod].Services.GetService(serviceType);
            }
            else {
                resolvedService = _rootServiceProvider.GetService(serviceType);
            }

            return resolvedService;
        }
    }
}
