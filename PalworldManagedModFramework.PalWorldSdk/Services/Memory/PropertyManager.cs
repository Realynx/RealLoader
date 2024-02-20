using PalworldManagedModFramework.Sdk.Attributes;
using System.Reflection;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class PropertyManager {
        private readonly ILogger _logger;

        public PropertyManager(ILogger logger) {
            _logger = logger;
        }

        private nint UpdatePropertyValue(PropertyInfo member, object? instance, MachineCodePatternAttribute machineCodeAttribute, nint patternOffsetAddress) {
            _logger.Debug($"{member.Name} pattern found: 0x{patternOffsetAddress:X}");

            var patternMask = ByteCodePattern.DeriveMask(machineCodeAttribute.Pattern);
            var resolvedAddress = _operandResolver.ResolveInstructionAddress(patternOffsetAddress, patternMask.OperandOffset, machineCodeAttribute);

            if (instance is not null) {
                if (machineCodeAttribute.PatternType == PatternType.Function) {
                    var delegateValue = Marshal
                        .GetDelegateForFunctionPointer(resolvedAddress, member.PropertyType);
                    member.SetValue(instance, delegateValue);
                }
                else {
                    member.SetValue(instance, resolvedAddress);
                }
            }

            return resolvedAddress;
        }
    }
}
