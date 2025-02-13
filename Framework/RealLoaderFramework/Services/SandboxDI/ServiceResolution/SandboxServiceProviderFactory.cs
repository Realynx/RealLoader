using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RealLoaderFramework.Services.SandboxDI.ServiceResolution {
    public class SandboxServiceProviderFactory : IServiceProviderFactory<IServiceCollection> {
        private readonly IServiceProvider _parentServiceProvider;
        private IServiceCollection _serviceDescriptors;

        public SandboxServiceProviderFactory(IServiceProvider parentServiceProvider) {
            _parentServiceProvider = parentServiceProvider ?? throw new ArgumentNullException(nameof(parentServiceProvider));
            _serviceDescriptors = new SandboxServiceCollection();
        }

        public IServiceCollection CreateBuilder(IServiceCollection services) {
            _serviceDescriptors = services;
            return _serviceDescriptors;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder) {
            _serviceDescriptors.Add(containerBuilder);

            return new SandboxServiceProvider(_parentServiceProvider, _serviceDescriptors);
        }
    }
}
