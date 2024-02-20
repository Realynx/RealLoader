using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace ExampleMod {
    [PalworldMod("Sample", "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Client)]
    public class Sample : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;

        public Sample(CancellationToken cancellationToken, ILogger logger) {
            _cancellationToken = cancellationToken;
            _logger = logger;
        }

        public void Load() {
            _logger.Info("UwU");
        }

        public void Unload() {
            _logger.Info("Unloading...");
        }

        [UnrealEvent("$BP_AIAction_CombatPal_C.*")]
        public unsafe void CombatEvent(UObject* instance, UFunction* uFunction, void* voidPtr) {

        }

        [HookEngineEvent("PalHate::DamageEvent")]
        public unsafe void CombatEvent(UObject* instance, UFunction* uFunction, void* voidPtr) {

        }
    }
}
