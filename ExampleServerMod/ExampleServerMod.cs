using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;

namespace ExampleServerMod {
    [PalworldMod("Sample", "poofyfox", ".poofyfox", "1.0.0", serverMod: true)]
    public class ExampleServerMod : IPalworldMod {
        private ILogger _logger;
        private CancellationToken _cancellationToken;

        public void Load(CancellationToken cancellationToken, ILogger logger) {
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        public void Unload() {

        }
    }
}
