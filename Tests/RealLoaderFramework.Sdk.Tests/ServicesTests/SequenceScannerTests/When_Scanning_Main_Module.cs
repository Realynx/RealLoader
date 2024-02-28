using System.Diagnostics;

using Xunit;

namespace RealLoaderFramework.Sdk.Tests.ServicesTests.SequenceScannerTests {
    public class When_Scanning_Main_Module : Using_Sequence_Scanner {
        private IntPtr[] _results = Array.Empty<IntPtr>();

        protected override void Setup() {
            SetupMemoryMapperService();
        }

        protected override void Act() {
            //_results = TestableImplementation.SequenceScan("48 8B 45 ?", Process.GetCurrentProcess().MainModule!);
        }

        [Fact]
        public void Did_Not_Throw() {
            Assert.NotNull(_results);
        }

        [Fact]
        public void DoesFindAnything() {
            Assert.NotEmpty(_results);
        }
    }
}
