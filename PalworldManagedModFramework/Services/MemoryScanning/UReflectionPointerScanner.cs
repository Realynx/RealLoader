using System.Runtime.InteropServices;

using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct UFunctionStruct {
        public string* uFunctionName;
        public void* uFunctionPointer;
    }

    internal unsafe class UReflectionPointerScanner {
        private readonly ILogger _logger;

        public UReflectionPointerScanner(ILogger logger) {
            _logger = logger;
        }

        public void ScanMemoryForUnrealReflectionPointers() {

            // DebugUtilities.WaitForDebuggerAttach();
            //var allPotentialReflectionStructs = SequenceScanner.SequenceScan("?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00");
            //_logger.Info($"Found {allPotentialReflectionStructs.Length} patterns OwO");


            //foreach (UFunctionStruct* uFunction in allPotentialReflectionStructs) {
            //    if (uFunction == null) {
            //        // Skip if the pointer is null
            //        continue;
            //    }

            //    if (uFunction->uFunctionName != null) {
            //        string functionName = new string(*uFunction->uFunctionName);
            //        _logger.Debug($"0x{(long)uFunction:x} - {functionName}");

            //        if (functionName == "SelfDeathEvent") {
            //            _logger.Info("Found Valid Struct!");
            //        }
            //    }
            //    else {
            //        _logger.Debug("uFunctionName is null");
            //    }
            //}

        }
    }
}
