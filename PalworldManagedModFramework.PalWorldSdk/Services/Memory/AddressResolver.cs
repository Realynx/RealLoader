using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    public class AddressResolver : IAddressResolver {
        private readonly ILogger _logger;

        public AddressResolver(ILogger logger) {
            _logger = logger;
        }

        public unsafe nint ResolveInstructionAddress(nint matchedAddress, MachineCodePatternAttribute patternAttribute) {
            switch (patternAttribute.PatternType) {
                case OperandType.DirectAddress_32:
                    var operandAddressValue = *(int*)(matchedAddress - 4);
                    return operandAddressValue;

                case OperandType.RelativeOffset_32:
                    var operandRelativeOffset = *(int*)(matchedAddress - 4);
                    return matchedAddress + operandRelativeOffset;
            }

            return 0;
        }
    }
}
