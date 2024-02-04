using System.Runtime.InteropServices;

using Xunit;

namespace PalworldManagedModFramework.PalWorldSdk.Tests.ServicesTests.SequenceScannerTests {
    public class When_Scanning_Region_With_Offset : Using_Sequence_Scanner {
        private const int TESTED_POSITION = 5000;
        private IntPtr[] _results = Array.Empty<IntPtr>();
        private nint _allocatedBytesAddress = 0;
        private readonly int _offset = 3;

        protected unsafe override void Setup() {
            SetupMemoryMapperService();

            memoryBytes = new byte[] {
                0x48, 0x8B, 0x05,
                0x00, 0x00, 0x00, 0x00,
                0x48, 0x8B, 0x0C, 0xC8, 0x4C, 0x8D, 0x04, 0xD1, 0xEB, 0x03
            };

            var allocatedBytes = (byte*)Marshal.AllocHGlobal(memoryBytes.Length + 10000);
            _allocatedBytesAddress = (nint)allocatedBytes;

            for (var x = 0; x < memoryBytes.Length; x++) {
                var memByte = memoryBytes[x];
                allocatedBytes[x + TESTED_POSITION] = memByte;
            }

            pattern = "48 8B 05 | ? ? ? ? 48 8B 0C C8 4C 8D 04 D1 EB 03";
        }

        protected override void Act() {
            _results = TestableImplementation.SequenceScan(pattern, _allocatedBytesAddress, _allocatedBytesAddress + (memoryBytes.Length + 10000));
        }

        [Fact]
        public unsafe void Does_Scanner_Find_Anything() {
            Assert.NotEmpty(_results);
        }

        [Fact]
        public unsafe void Does_Scanner_Find_Offset() {
            Assert.Contains(_results, i => i == _allocatedBytesAddress + TESTED_POSITION + _offset);
        }
    }
}
