using System.Diagnostics;

using PalworldManagedModFramework.PalWorldSdk.Services.Memory;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows;

using Shared.SystemUnderTest;

namespace PalworldManagedModFramework.PalWorldSdk.Tests.ServicesTests.MemoryMapperTests {
    public class Using_Windows_Memory_Mapper : SpecAutoMocker<IMemoryMapper, WindowsMemoryMapper> {
        protected ProcessModule? baseModule;

        public Using_Windows_Memory_Mapper() {
            Init();
        }
    }
}
