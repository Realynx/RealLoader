using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface IMemoryScanner {
        nint[]? SequenceScan(ByteCodePattern byteCodePattern);
        nint[]? SequenceScan(ByteCodePattern byteCodePattern, nint startAddress, nint endAddress);
        nint[][] SequenceScan(nint startAddress, nint endAddress, params ByteCodePattern[] byteCodePatterns);
        nint[][]? SequenceScan(params ByteCodePattern[] byteCodePatterns);
        nint? SingleSequenceScan(ByteCodePattern byteCodePattern);
    }
}