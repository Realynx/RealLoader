using Microsoft.Extensions.DependencyInjection;

namespace PalworldManagedModFramework.Services.SandboxDI.ServiceResolution {
    public class SandboxServiceProviderFactory : IServiceProviderFactory<IServiceCollection> {
        private readonly IServiceProvider _parentServiceProvider;
        private readonly IServiceProviderFactory<IServiceCollection> _defaultFactory;

        public SandboxServiceProviderFactory(IServiceProvider parentServiceProvider) {
            _parentServiceProvider = parentServiceProvider ?? throw new ArgumentNullException(nameof(parentServiceProvider));
            _defaultFactory = new DefaultServiceProviderFactory();
        }

        public IServiceCollection CreateBuilder(IServiceCollection services) {
            return _defaultFactory.CreateBuilder(services);
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder) {
            var childServiceProvider = _defaultFactory.CreateServiceProvider(containerBuilder);
            return new SandboxServiceProvider(_parentServiceProvider, childServiceProvider);
        }
    }
}
