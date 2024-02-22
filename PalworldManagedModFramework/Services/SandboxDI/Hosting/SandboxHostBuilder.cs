using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.Services.SandboxDI.ServiceResolution;

namespace PalworldManagedModFramework.Services.SandboxDI.Hosting {
    public class SandboxHostBuilder : IHostBuilder {
        private IServiceProviderFactory<IServiceCollection> _serviceProviderFactory;
        private readonly ConfigurationBuilder _configurationBuilder = new();
        private readonly HostBuilderContext _hostBuilderContext;
        private readonly IServiceCollection _services;

        public SandboxHostBuilder() {
            _hostBuilderContext = new HostBuilderContext(Properties);
            _services = new SandboxServiceCollection();
        }

        public IDictionary<object, object> Properties { get; protected set; } = new Dictionary<object, object>();

        public IHost Build() {
            if (_serviceProviderFactory == null) {
                throw new InvalidOperationException("Service provider factory is not configured.");
            }

            var serviceProvider = _serviceProviderFactory.CreateServiceProvider(_services);
            return new SandboxHost(serviceProvider);
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) {
            configureDelegate?.Invoke(_hostBuilderContext, _configurationBuilder);
            return this;
        }

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) {
            if (_serviceProviderFactory != null) {
                var containerBuilder = _serviceProviderFactory.CreateBuilder(_services);
                configureDelegate?.Invoke(_hostBuilderContext, (TContainerBuilder)containerBuilder);
            }

            return this;
        }

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate) {
            configureDelegate?.Invoke(new ConfigurationBuilder());
            return this;
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) {
            configureDelegate?.Invoke(_hostBuilderContext, _services);
            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull {
            _serviceProviderFactory = (IServiceProviderFactory<IServiceCollection>)factory;
            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull {
            _serviceProviderFactory = (IServiceProviderFactory<IServiceCollection>)factory(_hostBuilderContext);
            return this;
        }
    }
}
