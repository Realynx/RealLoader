using System.Reflection;

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
            var service = GetOrActivateService(serviceType);
            service ??= _rootServiceProvider.GetService(serviceType);

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
                    var service = Activator.CreateInstance(implementationType);
                    _serviceSingletons[serviceType] = service;
                    return service;
                }

                var arguments = GetConstructorArguments(DICtor);
                _serviceSingletons[serviceType] = DICtor.Invoke(arguments);
            }

            return _serviceSingletons[serviceType];
        }

        private object[] GetConstructorArguments(ConstructorInfo DICtor) {
            var parameters = DICtor.GetParameters();
            var arguments = new object[parameters.Length];
            for (var x = 0; x < parameters.Length; x++) {
                var parameter = parameters[x];

                arguments[x] = GetService(parameter.ParameterType);
            }

            return arguments;
        }

        public void Dispose() {
            if (_rootServiceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}
