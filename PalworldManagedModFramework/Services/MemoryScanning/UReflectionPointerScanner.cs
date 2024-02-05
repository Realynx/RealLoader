using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class UReflectionPointerScanner {
        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly IMemoryScanner _memoryScanner;

        public UReflectionPointerScanner(ILogger logger, IEnginePattern enginePattern, IMemoryScanner memoryScanner) {
            _logger = logger;
            _enginePattern = enginePattern;
            _memoryScanner = memoryScanner;
        }

        public void ScanMemoryForUnrealReflectionPointers() {
            // DebugUtilities.WaitForDebuggerAttach();
            _logger.Debug("Starting Pattern Scan");

            ScanForGlobalObjects();
            //ScanForNamesPool();
        }

        private unsafe void ScanForNamesPool() {
            var namePoolDataPattern = _memoryScanner.SingleSequenceScan(_enginePattern.NamePoolData);
            if (namePoolDataPattern is null) {
                _logger.Error($"Could not find {nameof(namePoolDataPattern)}!");
            }

            // subtract the 4 bytes that contains our relative offset int.
            var namePoolDataOffset = *(int*)(namePoolDataPattern - 4);
            // Wait until namePoolPointerPtr is set.
            var namePoolPtr = (nint)(namePoolDataPattern + namePoolDataOffset);
            _logger.Debug($"Found {nameof(_enginePattern.NamePoolData)} pattern. Pointer: 0x{namePoolPtr:X}");
        }

        private unsafe void ScanForGlobalObjects() {
            var guObjectsArrayPattern = _memoryScanner.SingleSequenceScan(_enginePattern.GUObjectArray);
            _logger.Debug($"Found Pattern {nameof(guObjectsArrayPattern)}: 0x{guObjectsArrayPattern:X}");

            if (guObjectsArrayPattern is null) {
                _logger.Error($"Could not find {nameof(guObjectsArrayPattern)}!");
                return;
            }

            // subtract the 4 bytes that contains our relative offset int. Also note these are signed offsets.
            var guObjectsArrayOffset = *(int*)(guObjectsArrayPattern - 4);
            _logger.Debug($"Found relative offset {nameof(guObjectsArrayOffset)}: 0x{guObjectsArrayOffset:X}");

            _logger.Debug($"{nameof(guObjectsArrayOffset)} Pointer Pointer: 0x{guObjectsArrayPattern + guObjectsArrayOffset:X}");

            //var test = *(nint*)guObjectsArrayOffset;
            //_logger.Debug($"Found value under offset: 0x{test:X}");
            // Wait until guObjectsArrayPointerPtr is set.
            //var guObjectsArrayPointerPtr = (nint*)(guObjectsArrayPattern + guObjectsArrayOffset);
            //while (*guObjectsArrayPointerPtr == 0x0) {
            //    Thread.Sleep(100);
            //}

            //var guObjectsArrayPtr = *(nint*)*guObjectsArrayPointerPtr;
            //_logger.Debug($"Found {nameof(_enginePattern.GUObjectArray)} pattern. Pointer: 0x{guObjectsArrayPtr:X}");
        }
    }
}
