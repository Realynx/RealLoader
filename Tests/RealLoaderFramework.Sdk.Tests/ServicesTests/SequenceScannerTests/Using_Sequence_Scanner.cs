﻿using System.Diagnostics;
using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Services.Memory.Linux;
using RealLoaderFramework.Sdk.Services.Memory.Windows;
using RealLoaderFramework.Sdk.Services.Memory;

using Shared.SystemUnderTest;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Logging;

namespace RealLoaderFramework.Sdk.Tests.ServicesTests.SequenceScannerTests {
    public class Using_Sequence_Scanner : SpecAutoMocker<ISequenceScanner, SequenceScanner> {
        protected byte[] memoryBytes = Array.Empty<byte>();
        protected string pattern = string.Empty;
        protected ProcessModule? scanningModule = null;

        public Using_Sequence_Scanner() {
            Init();
        }

        protected unsafe void SetupMemoryMapperService() {
            IMemoryMapper memoryMapper = null!;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                memoryMapper = new WindowsMemoryMapper();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                var mockedLogger = Mocker.CreateInstance<ILogger>();
                memoryMapper = new LinuxMemoryMapper(mockedLogger);
            }

            Mocker.Use(memoryMapper);
        }
    }
}
