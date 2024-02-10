using System.Diagnostics;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

using Xunit;

namespace PalworldManagedModFramework.Sdk.Tests.ServicesTests.MemoryMapperTests {
    public class When_Mapping_Windows_Memory : Using_Windows_Memory_Mapper {
        private MemoryRegion[] _memoryRegions = [];
        protected override void Setup() {

        }

        protected override void Act() {
            _memoryRegions = TestableImplementation.FindMemoryRegions();
        }

        [SkippableFact]
        public void Was_Not_Empty() {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            Assert.NotEmpty(_memoryRegions);
        }

        [SkippableFact]
        public void Found_Readable_Regions() {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            Assert.Contains(_memoryRegions, i => i.ReadFlag);
        }

        [SkippableFact]
        public void Found_Writeable_Regions() {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            Assert.Contains(_memoryRegions, i => i.WriteFlag);
        }

        [SkippableFact]
        public void Found_Executeable_Regions() {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            Assert.Contains(_memoryRegions, i => i.ExecuteFlag);
        }

        [SkippableFact]
        public unsafe void Found_Manually_Created_Memory() {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            var allocatedBytes = (int*)Marshal.AllocHGlobal(20);
            var address = (nint)allocatedBytes;

            Assert.Contains(_memoryRegions, i => address >= (nint)i.StartAddress && address < (nint)i.EndAddress);
            Marshal.FreeHGlobal((nint)allocatedBytes);
        }

        [SkippableFact]
        public unsafe void Found_Manually_Created_Memory_At_Non_Zero_Address() {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            var allocatedBytes = (int*)Marshal.AllocHGlobal(20);
            var address = (nint)allocatedBytes;

            var memoryRegion = _memoryRegions.SingleOrDefault(i => address >= (nint)i.StartAddress && address < (nint)i.EndAddress);

            Assert.True(memoryRegion.StartAddress != 0);
            Marshal.FreeHGlobal((nint)allocatedBytes);
        }

    }
}
