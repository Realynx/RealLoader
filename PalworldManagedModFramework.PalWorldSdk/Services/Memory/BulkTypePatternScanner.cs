using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class BulkTypePatternScanner {
        private readonly ILogger _logger;

        private readonly Dictionary<MemberInfo, ByteCodePattern> _registredPatterns = new();
        private readonly Dictionary<MemberInfo, object> _typeInstances = new();

        public BulkTypePatternScanner(ILogger logger) {
            _logger = logger;
        }

        public BulkTypePatternScanner RegisterProperty(PropertyInfo propertyInfo, object instance) {
            var patternAttribute = propertyInfo.GetCustomAttribute<MachineCodePatternAttribute>()
                ?? throw new Exception($"Property: {propertyInfo.Name} does not have a {nameof(MachineCodePatternAttribute)} attribute.");

            BuildPatternAndAdd(propertyInfo, patternAttribute.Pattern, instance);

            return this;
        }

        public BulkTypePatternScanner RegisterHook(MethodInfo methodInfo) {
            var patternAttribute = methodInfo.GetCustomAttribute<HookAttribute>()
                ?? throw new Exception($"Method: {methodInfo.Name} does not have a {nameof(HookAttribute)} attribute.");

            BuildPatternAndAdd(methodInfo, patternAttribute.Pattern);

            return this;
        }

        private void BuildPatternAndAdd(MemberInfo memberInfo, string patternString, object instance = null!) {
            var byteCodePattern = ByteCodePattern.DeriveMask(patternString);
            if (_registredPatterns.TryAdd(memberInfo, byteCodePattern)) {
                _typeInstances.TryAdd(memberInfo, instance);
            }
        }

        public void ScanAll() {

        }
    }
}
