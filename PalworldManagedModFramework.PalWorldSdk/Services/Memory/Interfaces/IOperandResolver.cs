using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IOperandResolver {
        nint ResolveInstructionAddress(nint matchedAddress, ByteCodePattern byteCodePattern);
    }
}