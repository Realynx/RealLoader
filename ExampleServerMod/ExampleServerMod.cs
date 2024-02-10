using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;

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
