using PalworldManagedModFramework.PalWorldSdk.Attributes;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces {
    public interface IAddressResolver {
        nint ResolveInstructionAddress(nint matchedAddress, MachineCodePatternAttribute patternAttribute);
    }
}