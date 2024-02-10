namespace PalworldManagedModFramework.Sdk.Services.Memory.Linux {
    public interface IMemoryScanner {
        nint[] SequenceScan(string signature);
        nint[] SequenceScan(string signature, nint startAddress, nint endAddress);
        nint? SingleSequenceScan(string signature);
    }
}