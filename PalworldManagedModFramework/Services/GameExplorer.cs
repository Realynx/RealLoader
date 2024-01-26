
using PalworldManagedModFramework.Services.Logging;

namespace PalworldManagedModFramework.Services {
    internal class GameExplorer {
        private readonly ILogger _logger;

        public GameExplorer(ILogger logger) {
            _logger = logger;
        }

        internal void Entry() {
            _logger.Info("Begin memory explorer...");
        }
    }
}
