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
        private readonly IEnginePattern _enginePattern;

        public UReflectionPointerScanner(ILogger logger, ISequenceScanner sequenceScanner, IEnginePattern enginePattern) {
            _logger = logger;
            _sequenceScanner = sequenceScanner;
            _enginePattern = enginePattern;
        }

        public void ScanMemoryForUnrealReflectionPointers() {
            DebugUtilities.WaitForDebuggerAttach();
            _logger.Debug("Starting Pattern Scan");

            var guObjectsArrayOffset = _sequenceScanner.SingleSequenceScan(_enginePattern.GUObjectArray);
            _logger.Debug($"Found {nameof(_enginePattern.GUObjectArray)} pattern. Offset: 0x{guObjectsArrayOffset:X}");

            var namePoolDataOffset = _sequenceScanner.SingleSequenceScan(_enginePattern.NamePoolData);
            _logger.Debug($"Found {nameof(_enginePattern.NamePoolData)} pattern. Offset: 0x{namePoolDataOffset:X}");
        }
    }
}
