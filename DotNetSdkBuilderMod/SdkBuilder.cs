using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod {
    [PalworldMod(nameof(SdkBuilder), "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class SdkBuilder : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger, IGlobalObjects globalObjects) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _globalObjects = globalObjects;
        }

        public void Load() {
            _logger.Debug("Loading SDK Builder!");

            var globalObjectPackages = _globalObjects
                .EnumerateNamedObjects()
                .Where(i => i.Key.StartsWith("/Engine"))
                .OrderBy(i => i.Key)
                .ToArray();

            foreach (var package in globalObjectPackages) {
                _logger.Debug($"{package.Key}");
            }

            _logger.Info($"Found {globalObjectPackages.Length} Object packages in the global objects pool!");
        }

        public void Unload() {

        }
    }
}
