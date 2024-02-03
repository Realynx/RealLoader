using System.Diagnostics;

using PalworldManagedModFramework.PalWorldSdk.Services;
using PalworldManagedModFramework.PalWorldSdk.Services.Interfaces;

using Shared.SystemUnderTest;

namespace PalworldManagedModFramework.PalWorldSdk.Tests.ServicesTests.SequenceScannerTests {
    public class Using_Sequence_Scanner : SpecAutoMocker<ISequenceScanner, SequenceScanner> {
        protected byte[] memoryBytes = Array.Empty<byte>();
        protected string pattern = string.Empty;
        protected ProcessModule? scanningModule = null;

        public Using_Sequence_Scanner() {
            Init();
        }
    }
}
