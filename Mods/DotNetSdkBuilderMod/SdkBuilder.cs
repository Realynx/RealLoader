using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Interfaces;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;

namespace DotNetSdkBuilderMod {
    [RealLoaderMod(nameof(SdkBuilder), ["scrubn", "poofyfox"], "", "1.0.0", RealLoaderModType.Universal)]
    public class SdkBuilder : IRealLoaderMod {
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

        [EngineEvent("^.*::OnInitialized")]
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
