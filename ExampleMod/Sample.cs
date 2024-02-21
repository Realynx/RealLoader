using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.UnrealHook.Interfaces;

using static PalworldManagedModFramework.Sdk.Services.UnrealHook.UnrealEvent;

namespace ExampleMod {
    [PalworldMod("Sample", "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Client)]
    public class Sample : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IUnrealHookManager _unrealHookManager;

        private readonly HashSet<string> _functions = new();

        public Sample(CancellationToken cancellationToken, ILogger logger, IUnrealHookManager unrealHookManager) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _unrealHookManager = unrealHookManager;
        }

        public void Load() {
            _unrealHookManager
                .RegisterUnrealHook(GetType().GetMethod(nameof(DamageEvent))!, this)
                .RegisterUnrealHook(GetType().GetMethod(nameof(JumpEvent))!, this)
                .RegisterUnrealEvent(GetType().GetMethod(nameof(CombatEvents))!, this)
                .RegisterUnrealEvent(GetType().GetMethod(nameof(AllEvents))!, this);
        }

        public void Unload() {
            _logger.Info("Unloading...");
        }

        [EngineEvent("^.*")]
        public unsafe void AllEvents(UnrealEvent unrealEvent) {
            if (_functions.Add(unrealEvent.EventName)) {
                // _logger.Debug($"Event: {unrealEvent.EventName}");
            }
        }

        [EngineEvent("^BP_PalPlayerController_C.*")]
        public unsafe void CombatEvents(UnrealEvent unrealEvent) {
            // _logger.Warning($"Event: {unrealEvent.EventName}");

        }

        [HookEngineEvent("BP_Player_Female_C::CanJumpInternal")]
        public unsafe void JumpEvent(UnrealEvent unrealEvent, ExecuteOriginalCallback executeOriginalCallback) {
            _logger.Debug("Jumping Enabled!");

            executeOriginalCallback(unrealEvent.Instance, unrealEvent.UFunction, unrealEvent.Params);

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
