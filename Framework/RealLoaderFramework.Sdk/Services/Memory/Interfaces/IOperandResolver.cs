using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface IOperandResolver {
        nint ResolveInstructionAddress(nint matchedAddress, ByteCodePattern byteCodePattern);
    }
}