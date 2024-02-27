using System.Reflection;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class BulkPatternScanner : IBulkPatternScanner {
        private readonly ILogger _logger;
        private readonly IMemoryScanner _memoryScanner;
        private readonly IOperandResolver _operandResolver;
        private readonly IPropertyManager _propertyManager;

        private readonly Dictionary<MemberInfo, ByteCodePattern> _registeredPatterns = new();
        private readonly Dictionary<ByteCodePattern, nint> _matchedPatterns = new();
        private readonly Dictionary<MemberInfo, object> _typeInstances = new();

        public BulkPatternScanner(ILogger logger, IMemoryScanner memoryScanner, IOperandResolver operandResolver, IPropertyManager propertyManager) {
            _logger = logger;
            _memoryScanner = memoryScanner;
            _operandResolver = operandResolver;
            _propertyManager = propertyManager;
        }

        public MemberInfo[] GetAllRegisteredMembers() {
            return _registeredPatterns.Keys.ToArray();
        }

        public ByteCodePattern GetRegisteredByteCode(MemberInfo memberInfo) {
            return _registeredPatterns[memberInfo];
        }

        public nint? GetMatchedAddress(ByteCodePattern byteCodePattern) {
            if (_matchedPatterns.TryGetValue(byteCodePattern, out var address)) {
                return address;
            }

            return null;
        }

        public object GetRegisteredTypeInstance(MemberInfo memberInfo) {
            return _typeInstances[memberInfo];
        }

        public bool AddPattern(MemberInfo memberInfo, ByteCodePattern byteCodePattern, object instance = null!) {
            if (_registeredPatterns.TryAdd(memberInfo, byteCodePattern)) {
                _typeInstances.TryAdd(memberInfo, instance);
                return true;
            }

            _logger.Error($"Tried to add duplicate {nameof(ByteCodePattern)} for {memberInfo.Name}.");
            return false;
        }

        public IBulkPatternScanner ScanAll() {
            var allPatterns = _registeredPatterns.Values.Where(x => !_matchedPatterns.ContainsKey(x)).ToArray();
            var matchedPatterns = _memoryScanner.SequenceScan(allPatterns)
                ?? throw new Exception("Could not find any patterns!");

            for (var x = 0; x < allPatterns.Length; x++) {
                var pattern = allPatterns[x];
                var matchedAddresses = matchedPatterns[x];

                if (matchedAddresses.Length > 0) {
                    _matchedPatterns.Add(pattern, matchedAddresses[0]);
                }
            }

            return this;
        }
    }
}
