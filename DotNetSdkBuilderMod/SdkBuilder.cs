using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.Flags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod {
    [PalworldMod(nameof(SdkBuilder), "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class SdkBuilder : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly ISourceCodeGenerator _sourceCodeGenerator;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger, ISourceCodeGenerator sourceCodeGenerator) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _sourceCodeGenerator = sourceCodeGenerator;
        }

        public unsafe void Load() {
            _logger.Debug("Loading SDK Builder!");

            _logger.Debug("Waiting for classes to load into memory...");
            Thread.Sleep(TimeSpan.FromSeconds(7));
            _logger.Debug("Done sleeping");

            _sourceCodeGenerator.BuildSourceCode();

            // Debuging hook functions
        }


        [Hook("48 89 8E 80 00 ? ? 48 89 8E 88 00 ? ? 48 89 8E 90 00 ? ? 48 89 8E 98 00 ? ? 48 89 8E A0 00 ? ? 48 89 8E A8 00 ? ? 48 8B 74 ? ? | 48 83 ? ? ? C3", execute: true, overideReturn: false)]
        public static unsafe void UStructCtorHook(UStruct* instance, int ctorFlags, int param2, int param3, EObjectFlags param4) {
            _logger.Info($"Constructed 0x{(nint)instance:X}");
        }

        public void Unload() {

        }
    }
}
