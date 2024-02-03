using System.Diagnostics;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.PalWorldSdk.Services.Memory;

using Xunit;

namespace PalworldManagedModFramework.PalWorldSdk.Tests.ServicesTests.MemoryMapperTests {
    public class When_Mapping_Windows_Memory : Using_Windows_Memory_Mapper {
        private MemoryRegion[] _memoryRegions = [];
        protected override void Setup() {
            baseModule = Process.GetCurrentProcess().MainModule;
        }

        protected override void Act() {
            _memoryRegions = TestableImplementation.FindMemoryRegions(baseModule!);
        }

        [SkippableFact]
        public void Did_Fill_Valid_Data() {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            Assert.Equal(baseModule!.BaseAddress, (nint)_memoryRegions.First().StartAddress);
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

        //[SkippableFact]
        //public unsafe void Found_Manually_Created_Memory() {
        //    Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

        //    var allocatedBytes = (int*)Marshal.AllocHGlobal(20);
        //    var address = (nint)allocatedBytes;

        //    Assert.Contains(_memoryRegions, i => address >= (nint)i.StartAddress && address < (nint)i.EndAddress);


        //    Marshal.FreeHGlobal((nint)allocatedBytes);
        //}

    }
}
