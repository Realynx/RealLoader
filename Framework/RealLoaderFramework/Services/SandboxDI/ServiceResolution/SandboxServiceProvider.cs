using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace RealLoaderFramework.Services.SandboxDI.ServiceResolution {
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

            if (service is null) {
                throw new Exception($"Could not resolve dependency {serviceType.FullName}.");
            }

            return service;
        }

        private object? GetOrActivateService(Type serviceType) {
            if (_serviceSingletons.TryGetValue(serviceType, out var service)) {
                return service;
            }

            var existingType = _sandboxServiceProvider.FirstOrDefault(i => i.ServiceType == serviceType);
            if (existingType is null) {
                return null;
            }

            var implementationType = existingType.ImplementationType;
            if (implementationType is null) {
                _serviceSingletons[serviceType] = existingType.ImplementationInstance;
                return existingType.ImplementationInstance;
            }

            var constructors = implementationType.GetConstructors();
            var DICtor = constructors.MaxBy(i => i.GetParameters().Length);

            if (DICtor is null) {
                service = Activator.CreateInstance(implementationType);
                _serviceSingletons[serviceType] = service;
                return service;
            }

            var arguments = GetConstructorArguments(DICtor);
            service = DICtor.Invoke(arguments);
            _serviceSingletons[serviceType] = service;
            return service;
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
