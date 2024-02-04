using System.Diagnostics;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.PalWorldSdk.Services.Memory;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows;

using Xunit;

namespace PalworldManagedModFramework.PalWorldSdk.Tests.ServicesTests.SequenceScannerTests {
    public class When_Scanning_Perfect_Region_No_Offset_Pattern : Using_Sequence_Scanner {
        private IntPtr[] _results = Array.Empty<IntPtr>();
        private nint allocatedBytesAddress = 0;

        protected unsafe override void Setup() {
            SetupMemoryMapperService();

            memoryBytes = new byte[] { 0x1A, 0xBC, 0x2F, 0x7D, 0x4E, 0x88, 0x3C, 0x5A };
            var allocatedBytes = (byte*)Marshal.AllocHGlobal(memoryBytes.Length);
            allocatedBytesAddress = (nint)allocatedBytes;

            for (var x = 0; x < memoryBytes.Length; x++) {
                var memByte = memoryBytes[x];
                allocatedBytes[x] = memByte;
            }

            pattern = "1A BC 2F 7D 4E 88 3C 5A";
        }

        protected override void Act() {
            _results = TestableImplementation.SequenceScan(pattern, allocatedBytesAddress, allocatedBytesAddress + memoryBytes.Length);
        }

        [Fact]
        public unsafe void Does_Scanner_Find_Anything() {
            Assert.NotEmpty(_results);
        }

        [Fact]
        public unsafe void Does_Scanner_Find_Single() {
            Assert.Single(_results);
        }

        [Fact]
        public unsafe void Does_Scanner_Find_Block() {
            Assert.Contains(_results, i => i == allocatedBytesAddress);
        }
    }
}
