using System.Reflection;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class PropertyManager : IPropertyManager {
        private readonly ILogger _logger;
        private readonly IOperandResolver _operandResolver;

        public PropertyManager(ILogger logger, IOperandResolver operandResolver) {
            _logger = logger;
            _operandResolver = operandResolver;
        }

        public void UpdatePropertyValue(ByteCodePattern byteCodePattern, nint address, PropertyInfo member, object? instance) {
            _logger.Debug($"{member.Name} Property Value Set: 0x{address:X}");

            var resolvedAddress = _operandResolver.ResolveInstructionAddress(address, byteCodePattern);
            if (instance is not null) {
                if (byteCodePattern.PatternType == PatternType.Function) {
                    var delegateValue = Marshal
                        .GetDelegateForFunctionPointer(resolvedAddress, member.PropertyType);

                    member.SetValue(instance, delegateValue);
                }
                else {
                    member.SetValue(instance, resolvedAddress);
                }
            }
        }
    }
}
