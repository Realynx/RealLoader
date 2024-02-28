using GeneratedSdk.Game.Pal.Blueprint.Character.Base.BP_PlayerBase;
using GeneratedSdk.Script.Engine;

using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Interfaces;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.EngineServices;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;

using static RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook.UnrealEvent;

namespace ExampleMod {
    [RealLoaderMod("Sample", "poofyfox", ".poofyfox", "1.0.0", RealLoaderModType.Universal)]
    public class Sample : IRealLoaderMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IUnrealEventRegistrationService _unrealEventRegistrationService;
        private readonly IStackWalker _stackWalker;
        private readonly UObjectFactory _uObjectFactory;
        private readonly HashSet<string> _functions = new();

        public Sample(CancellationToken cancellationToken, ILogger logger, IUnrealEventRegistrationService unrealEventRegistrationService, IStackWalker stackWalker, UObjectFactory uObjectFactory) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _unrealEventRegistrationService = unrealEventRegistrationService;
            _stackWalker = stackWalker;
            _uObjectFactory = uObjectFactory;
        }

        public void Load() {
            _unrealEventRegistrationService
                .FindAndRegisterEvents<Sample>(this)
                .FindAndRegisterEventHooks<Sample>(this);
        }

        public void Unload() {
            _logger.Info("Unloading...");
        }

        //[EngineEvent("^.*")]
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public unsafe void AllEvents(UnrealEvent unrealEvent) {
        //    if (_functions.Add(unrealEvent.EventName)) {
        //        _logger.Debug($"Event: {unrealEvent.EventName}");
        //    }
        //}

        [EngineEvent(".*PinkCat.*")]
        public unsafe void PinkCatEvents(UnrealEvent unrealEvent) {
            _logger.Error($"Event: {unrealEvent.EventName}");
        }


        [HookEngineEvent("BP_Player_Female_C::CanJumpInternal")]
        public unsafe void JumpEvent(UnrealEvent unrealEvent, ExecuteOriginalCallback executeOriginalCallback) {
            try {
                var bitchPlayer = UObjectFactory.CreateProxy<BP_Player_Female_C>(unrealEvent.Instance);
                bitchPlayer.Jump();

                _logger.Debug("Bitch Jumpin 🧨🧨🧨🧨🎈🎈🎃🎃🎃🎃📎🖇📏📐✂🗃🗄🗑📌📌📊📊📊📊📊");
            }
            catch {
                _logger.Debug("Bitch not be jumpin (┬┬﹏┬┬)");
            }

            unrealEvent.ContinueExecute = false;
        }

        [HookEngineEvent("PalHate::DamageEvent")]
        public unsafe void DamageEvent(UnrealEvent unrealEvent, ExecuteOriginalCallback executeOriginalCallback) {
            _logger.Debug("Pal was damaged");

            //executeOriginalCallback(unrealEventRecord.Instance, unrealEventRecord.UFunction, unrealEventRecord.Params);
            unrealEvent.ContinueExecute = false;
        }
    }
}
