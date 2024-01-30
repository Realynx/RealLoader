using System.Diagnostics;

using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services;

namespace ExampleMod {
    [PalworldMod("Sample", "poofyfox", ".poofyfox", "1.0.0")]
    public unsafe class Sample : IPalworldMod {
        private CancellationToken _cancellationToken;
        private ILogger _logger;

        public void Load(CancellationToken cancellationToken, ILogger logger) {
            _cancellationToken = cancellationToken;
            _logger = logger;

            while (!Debugger.IsAttached) {
                Thread.Sleep(100);
            }

            // Windows Bytecode Pattern
            var gameObjectManagerPtr = SequenceScanner.SingleSequenceScan("49 ?? ?? ?? 48 ?? ?? ?? ?? ?? ?? 49 ?? ?? ?? f6");

            var allPotentialReflectionStructs = SequenceScanner.SingleSequenceScan("?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? 00 00 00");

            var gobalWorldAddress = gameObjectManagerPtr + 5;
            _logger.Info($"Found Global World Instance: 0x{gobalWorldAddress:X}");


            SequenceScanner.SingleSequenceScan("");

            //UnrealHook.HookFunction<int>("/Engine/Pal/SomeClass:FunctionName", (rax => {
            //    return 5;
            //}));

            //UnrealHook.HookFunction<int>("/Engine/Pal/SomeClass:FunctionName", (rax => {
            //    _logger.Info($"[FunctionName] was executed with {rax}");
            //}), true);

            _logger.Info("Hello World!");
        }

        public void Unload() {
            _logger.Info("Unloading...");
        }
    }
}
