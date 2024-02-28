using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Interfaces;
using RealLoaderFramework.Sdk.Logging;

namespace ExampleServerMod {
    [RealLoaderMod("Sample", "poofyfox", ".poofyfox", "1.0.0", RealLoaderModType.Server)]
    public class ExampleServerMod : IRealLoaderMod {
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
