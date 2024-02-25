using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Extensions;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
    public class PropertyRegistrationService : IPropertyRegistrationService {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBulkPatternScanner _bulkPatternScanner;

        public PropertyRegistrationService(ILogger logger, IServiceProvider serviceProvider, IBulkPatternScanner bulkPatternScanner) {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _bulkPatternScanner = bulkPatternScanner;
        }

        public IPropertyRegistrationService FindAndRegisterProperties<TType>() {
            var parentType = typeof(TType);
            var parentInstance = _serviceProvider.GetService(parentType);
            if (parentInstance is null) {
                return this;
            }

            // Get the implementation if we have an interface from TType.
            parentType = parentInstance.GetType();

            var validProperties = GetValidProperties(parentType);
            foreach (var validProperty in validProperties) {
                _bulkPatternScanner.RegisterProperty(validProperty, parentInstance);
            }

            return this;
        }

        private IEnumerable<PropertyInfo> GetValidProperties(Type parentType) {
            var properties = parentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties) {
                var machineCodePatternAttribute = property.GetCustomAttribute<MachineCodePatternAttribute>();
                if (machineCodePatternAttribute is null) {
                    continue;
                }

                yield return property;
            }
        }
    }
}
