using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class OperandResolver : IOperandResolver {
        private readonly ILogger _logger;
        private readonly IMemoryMapper _memoryMapper;

        public OperandResolver(ILogger logger, IMemoryMapper memoryMapper) {
            _logger = logger;
            _memoryMapper = memoryMapper;
        }

        public unsafe nint ResolveInstructionAddress(nint matchedAddress, ByteCodePattern byteCodePattern) {
            switch (byteCodePattern.PatternType) {
                case PatternType.DirectAddress_32:
                    var operandAddressValue = *(int*)(matchedAddress - 4);
                    return operandAddressValue;

                case PatternType.IP_RelativeOffset_32:
                    var operandRelativeOffset = *(int*)(matchedAddress - 4);
                    return matchedAddress + operandRelativeOffset;

                // TODO: This is untested and possibly YAGNI
                case PatternType.Base_RelativeOffset_32:
                    operandRelativeOffset = *(int*)(matchedAddress - 4);
                    var baseAddress = _memoryMapper.GetBaseAddress();
                    return baseAddress + operandRelativeOffset;

                case PatternType.Function:
                    return matchedAddress;
            }

            return 0;
        }
    }
}
