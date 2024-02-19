using System.Reflection;

using PalworldManagedModFramework.Sdk.Logging;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class BulkPatternScanner {
        private readonly ILogger _logger;

        private readonly Dictionary<>

        public BulkPatternScanner(ILogger logger) {
            _logger = logger;
        }

        public BulkPatternScanner RegisterProperty(PropertyInfo propertyInfo, object instance) {

        }

        public BulkPatternScanner RegisterHook(MethodInfo methodInfo) {

        }
    }
}
