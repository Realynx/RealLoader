using System.Diagnostics;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Interfaces {
    public interface ISequenceScanner {
        nint[] SequenceScan(string signature);
        nint[] SequenceScan(string signature, ProcessModule processModule);
        nint? SingleSequenceScan(string signature);
    }
}