using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;

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
            _logger.Info("Hello World!");
        }

        public void Unload() {
            _logger.Info("Unloading...");
        }
    }
}
