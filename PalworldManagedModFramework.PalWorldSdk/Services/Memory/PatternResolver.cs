using PalworldManagedModFramework.PalWorldSdk.Attributes;
using System.Reflection;

using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    public class PatternResolver : IPatternResolver {
        private readonly ILogger _logger;
        private readonly IAddressResolver _addressResolver;
        private readonly IMemoryScanner _memoryScanner;

        public PatternResolver(ILogger logger, IAddressResolver addressResolver, IMemoryScanner memoryScanner) {
            _logger = logger;
            _addressResolver = addressResolver;
            _memoryScanner = memoryScanner;
        }

        public nint? ResolvePattern(PropertyInfo member, object? instance = null) {
            var machineCodeAttribute = member.GetCustomAttributes<MachineCodePatternAttribute>(true).FirstOrDefault()
                ?? throw new InvalidOperationException($"Type must have {nameof(MachineCodePatternAttribute)}");

            var patternOffsetAddress = _memoryScanner.SingleSequenceScan(machineCodeAttribute.Pattern);
            if (patternOffsetAddress is null) {
                _logger.Error($"Could not resolve pattern for {member.Name}, Pattern: {machineCodeAttribute.Pattern}");

                return null;
            }

            var resolvedAddress = _addressResolver.ResolveInstructionAddress((nint)patternOffsetAddress, machineCodeAttribute);
            if (instance is not null) {
                member.SetValue(instance, resolvedAddress);
            }

            return resolvedAddress;
        }
    }
}
