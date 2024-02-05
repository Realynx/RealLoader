using System.Diagnostics;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory;

using Shared.SystemUnderTest;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.PalWorldSdk.Tests.ServicesTests.SequenceScannerTests {
    public class Using_Sequence_Scanner : SpecAutoMocker<ISequenceScanner, SequenceScanner> {
        protected byte[] memoryBytes = Array.Empty<byte>();
        protected string pattern = string.Empty;
        protected ProcessModule? scanningModule = null;

        public Using_Sequence_Scanner() {
            Init();
        }

        protected unsafe void SetupMemoryMapperService() {
            IMemoryMapper memoryMapper = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                memoryMapper = new WindowsMemoryMapper();
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix) {
                memoryMapper = new LinuxMemoryMapper();
            }

            Mocker.Use(memoryMapper!);
        }
    }
}
