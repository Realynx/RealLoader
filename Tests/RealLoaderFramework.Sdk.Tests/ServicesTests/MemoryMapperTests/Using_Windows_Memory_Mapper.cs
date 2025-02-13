using System.Diagnostics;

using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Windows;

using Shared.SystemUnderTest;

namespace RealLoaderFramework.Sdk.Tests.ServicesTests.MemoryMapperTests {
    public class Using_Windows_Memory_Mapper : SpecAutoMocker<IMemoryMapper, WindowsMemoryMapper> {
        protected ProcessModule? baseModule;

        public Using_Windows_Memory_Mapper() {
            Init();
        }
    }
}
