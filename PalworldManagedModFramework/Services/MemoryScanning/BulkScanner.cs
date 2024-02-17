using System.Reflection;

using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class BulkScanner : IDisposable {
        private readonly Dictionary<PropertyInfo, object> _patternProperties = new();
        private readonly IPatternResolver _patternResolver;

        public BulkScanner(IPatternResolver patternResolver) {
            _patternResolver = patternResolver;
        }

        public BulkScanner ResolveMachineCodeProperty(object instance, string propertyToResolve) {
            var propInfo = instance.GetType().GetProperty(propertyToResolve);
            _patternProperties.TryAdd(propInfo, instance);

            return this;
        }

        public nint?[] ScanAllProperties() {
            return _patternResolver.ResolvePatterns(_patternProperties.Keys.ToArray(), _patternProperties.Values.ToArray());
        }

        public void Dispose() {
            _patternProperties.Clear();
        }
    }
}
