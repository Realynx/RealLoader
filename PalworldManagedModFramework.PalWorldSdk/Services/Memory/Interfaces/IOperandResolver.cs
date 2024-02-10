using PalworldManagedModFramework.Sdk.Attributes;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IOperandResolver {
        nint ResolveInstructionAddress(nint matchedAddress, nint operandOffset, MachineCodePatternAttribute patternAttribute);
    }
}