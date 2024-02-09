using PalworldManagedModFramework.PalWorldSdk.Attributes;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces {
    public interface IOperandResolver {
        nint ResolveInstructionAddress(nint matchedAddress, nint operandOffset, MachineCodePatternAttribute patternAttribute);
    }
}