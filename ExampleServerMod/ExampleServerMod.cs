using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;

namespace ExampleServerMod {
    [PalworldMod("Sample", "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Server)]
    public class ExampleServerMod : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;

        public ExampleServerMod(CancellationToken cancellationToken, ILogger logger) {
            _cancellationToken = cancellationToken;
            _logger = logger;
        }

        public void Load() {

        }

        public void Unload() {

        }
    }
}
