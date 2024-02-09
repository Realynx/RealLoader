using PalworldManagedModFramework.Models.Config;
using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning;

namespace PalworldManagedModFramework.Services.AssemblyLoading {
    internal class ModLoader : IModLoader {
        private readonly ILogger _logger;
        private readonly IAssemblyDiscovery _assemblyDiscovery;
        private readonly IServiceProvider _serviceProvider;
        private readonly ModLoaderConfig _modLoaderConfig;
        private readonly UReflectionPointerScanner _uReflectionPointerScanner;

        public ModLoader(ILogger logger, IAssemblyDiscovery assemblyDiscovery, IServiceProvider serviceProvider,
            ModLoaderConfig modLoaderConfig, UReflectionPointerScanner uReflectionPointerScanner) {
            _logger = logger;
            _assemblyDiscovery = assemblyDiscovery;
            _serviceProvider = serviceProvider;
            _modLoaderConfig = modLoaderConfig;
            _uReflectionPointerScanner = uReflectionPointerScanner;
        }

        public void LoadMods() {
            var loadableMods = PalworldModType.Universal;
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                loadableMods |= PalworldModType.Server;

                _logger.Info("Detected Server environment. Loading Server and Universal mods only.");
            }
            else {
                loadableMods |= PalworldModType.Client;
            }

            if (!_modLoaderConfig.EnableMods) {
                _logger.Warning("Mod loading has been disabled in config! Not loading any mods");
                return;
            }

            ScanRuntime();

            _logger.Info("Loading mods");
            var validMods = _assemblyDiscovery.DiscoverValidModAsselblies();

            foreach (var mod in validMods) {
                if (_modLoaderConfig.EnableModWhiteList &&
                    !_modLoaderConfig.ModWhiteList.Contains(mod.PalworldModAttribute.ModName)) {
                    continue;
                }

                if (!loadableMods.HasFlag(mod.PalworldModAttribute.ModType)) {
                    _logger.Warning($"Skipping incompatable mod: [{mod.PalworldModAttribute.ModName}]");
                    continue;
                }

                _logger.Info($"[Loading] Mod: {mod.PalworldModAttribute.ModName}, Author: {mod.PalworldModAttribute.Author}");
                var modTypes = mod.Assembly.GetTypes();
                var modEntryPoint = modTypes.FirstOrDefault(i => i.GetInterface(nameof(IPalworldMod)) is not null);

                if (modEntryPoint is null) {
                    _logger.Error($"[{mod.PalworldModAttribute.ModName}] No valid mod entry point was found! " +
                        $"You should contact ({mod.PalworldModAttribute.Author}) {mod.PalworldModAttribute.DiscordAlias} in discord.");
                    continue;
                }

                var loadedMod = new LoadedMod(modEntryPoint, mod, _logger, _serviceProvider);
            }
        }

        internal void ScanRuntime() {
            _logger.Debug("Scanning for reflected functions.");
            _uReflectionPointerScanner.ScanMemoryForUnrealReflectionPointers();
        }
    }
}
