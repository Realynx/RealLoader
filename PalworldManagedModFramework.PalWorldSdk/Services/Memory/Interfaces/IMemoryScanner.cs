namespace PalworldManagedModFramework.Sdk.Services.Memory.Linux {
    public interface IMemoryScanner {
        nint[]? SequenceScan(string signature);
        nint[]? SequenceScan(string signature, nint startAddress, nint endAddress);
        nint[][] SequenceScan(nint startAddress, nint endAddress, params string[] signature);
        nint[][]? SequenceScan(params string[] signature);
        nint? SingleSequenceScan(string signature);
    }
}