using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;

namespace DotNetSdkBuilderMod {
    [PalworldMod(nameof(SdkBuilder), "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class SdkBuilder : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly ISourceCodeGenerator _sourceCodeGenerator;
        private readonly IUnrealEventRegistrationService _unrealEventRegistrationService;
        private bool _objectsReady = false;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger, ISourceCodeGenerator sourceCodeGenerator, IUnrealEventRegistrationService unrealEventRegistrationService) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _sourceCodeGenerator = sourceCodeGenerator;
            _unrealEventRegistrationService = unrealEventRegistrationService;
        }

        public unsafe void Load() {
            _logger.Debug("Loading SDK Builder!");

            _unrealEventRegistrationService
                .FindAndRegisterEvents<SdkBuilder>(this);
        }

        [EngineEvent("^WBP_TItle_C::OnInitialized")]
        public unsafe void ObjectsReady(UnrealEvent unrealEvent) {
            if (!_objectsReady) {
                _objectsReady = true;

                _logger.Info($"Objects are ready. Building .NET SDK");
                _sourceCodeGenerator.BuildSourceCode();
            }

        }

        public void Unload() {

        }
    }
}
