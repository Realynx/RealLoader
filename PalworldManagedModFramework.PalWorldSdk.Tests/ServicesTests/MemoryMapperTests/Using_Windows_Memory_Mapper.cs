using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Windows;

using Shared.SystemUnderTest;

namespace PalworldManagedModFramework.Sdk.Tests.ServicesTests.MemoryMapperTests {
    public class Using_Windows_Memory_Mapper : SpecAutoMocker<IMemoryMapper, WindowsMemoryMapper> {
        protected ProcessModule? baseModule;

        public Using_Windows_Memory_Mapper() {
            Init();
        }
    }
}
