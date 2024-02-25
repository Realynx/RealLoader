﻿using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

using static PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook.UnrealEvent;

namespace ExampleMod {
    [PalworldMod("Sample", "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class Sample : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IUnrealEventRegistrationService _unrealEventRegistrationService;
        private readonly IStackWalker _stackWalker;
        private readonly HashSet<string> _functions = new();

        public Sample(CancellationToken cancellationToken, ILogger logger, IUnrealEventRegistrationService unrealEventRegistrationService, IStackWalker stackWalker) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _unrealEventRegistrationService = unrealEventRegistrationService;
            _stackWalker = stackWalker;
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
            _logger.Debug("Jumping Disabled!");
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
