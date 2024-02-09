using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    public class OperandResolver : IOperandResolver {
        private readonly ILogger _logger;
        private readonly IMemoryMapper _memoryMapper;

        public OperandResolver(ILogger logger, IMemoryMapper memoryMapper) {
            _logger = logger;
            _memoryMapper = memoryMapper;
        }

        public unsafe nint ResolveInstructionAddress(nint matchedAddress, nint operandOffset, MachineCodePatternAttribute patternAttribute) {
            switch (patternAttribute.PatternType) {
                case OperandType.DirectAddress_32:
                    var operandAddressValue = *(int*)(matchedAddress + operandOffset - 4);
                    return operandAddressValue;

                case OperandType.IP_RelativeOffset_32:
                    var operandRelativeOffset = *(int*)(matchedAddress + operandOffset - 4);
                    return matchedAddress + operandOffset + operandRelativeOffset;

                // TODO: This is untested and possibly YAGNI
                case OperandType.Base_RelativeOffset_32:
                    operandRelativeOffset = *(int*)(matchedAddress + operandOffset - 4);
                    var baseAddress = _memoryMapper.GetBaseAddress();
                    return baseAddress + operandRelativeOffset;

                case OperandType.Function:
                    return matchedAddress + operandOffset;
            }

            return 0;
        }
    }
}
