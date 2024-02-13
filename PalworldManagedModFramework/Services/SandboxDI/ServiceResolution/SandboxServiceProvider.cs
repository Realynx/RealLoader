using Microsoft.Extensions.DependencyInjection;

namespace PalworldManagedModFramework.Services.SandboxDI.ServiceResolution {
    public class SandboxServiceProvider : IServiceProvider, IDisposable {
        private readonly IServiceProvider _rootServiceProvider;
        private readonly IServiceCollection _sandboxServiceProvider;

        private readonly Dictionary<Type, object> _serviceSingletons = new();

        public SandboxServiceProvider(IServiceProvider root, IServiceCollection sandbox) {
            _rootServiceProvider = root;
            _sandboxServiceProvider = sandbox;
        }

        public object GetService(Type serviceType) {
            var service = _rootServiceProvider.GetService(serviceType);
            service ??= GetOrActivateService(serviceType);

            return service;
        }

        private object? GetOrActivateService(Type serviceType) {
            if (!_serviceSingletons.ContainsKey(serviceType)) {
                var existingType = _sandboxServiceProvider.FirstOrDefault(i => i.ServiceType == serviceType);
                if (existingType is null) {
                    return null;
                }

                var implementationType = existingType.ImplementationType;
                var constructors = implementationType.GetConstructors();
                var DICtor = constructors.OrderByDescending(i => i.GetParameters().Length).FirstOrDefault();

                if (DICtor is null) {
                    _serviceSingletons[serviceType] = Activator.CreateInstance(implementationType);
                }

                var parameters = DICtor.GetParameters();
                var arguments = new object[parameters.Length];
                for (var x = 0; x < parameters.Length; x++) {
                    var parameter = parameters[x];

                    arguments[x] = GetService(parameter.ParameterType);
                }

                _serviceSingletons[serviceType] = DICtor.Invoke(arguments);
            }

            return _serviceSingletons[serviceType];
        }

        public void Dispose() {
            if (_rootServiceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}
