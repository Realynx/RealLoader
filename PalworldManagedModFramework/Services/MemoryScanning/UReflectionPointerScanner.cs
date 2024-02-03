using System.Runtime.InteropServices;

using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services;
using PalworldManagedModFramework.PalWorldSdk.Services.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning.EnginePatterns;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct UFunctionStruct {
        public string* uFunctionName;
        public void* uFunctionPointer;
    }

    internal unsafe class UReflectionPointerScanner {
        private readonly ILogger _logger;
        private readonly ISequenceScanner _sequenceScanner;

        public UReflectionPointerScanner(ILogger logger, ISequenceScanner sequenceScanner) {
            _logger = logger;
            _sequenceScanner = sequenceScanner;
        }

        public void ScanMemoryForUnrealReflectionPointers() {

            IEnginePattern enginePatterns = Environment.OSVersion.Platform == PlatformID.Unix
                ? new LinuxServerPattern()
                : new WindowsClientPattern();


            DebugUtilities.WaitForDebuggerAttach();

            var guObjectsArrayOffset = _sequenceScanner.SequenceScan(enginePatterns.GUObjectArray);
            _logger.Info($"Found {nameof(enginePatterns.GUObjectArray)} pattern.");

            var namePoolDataOffset = _sequenceScanner.SequenceScan(enginePatterns.NamePoolData);
            _logger.Info($"Found {nameof(enginePatterns.NamePoolData)} pattern.");
        }
    }
}
