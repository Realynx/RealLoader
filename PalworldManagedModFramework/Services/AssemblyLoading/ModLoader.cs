using PalworldManagedModFramework.Models.Config;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning;

namespace PalworldManagedModFramework.Services.AssemblyLoading {
    internal class ModLoader : IModLoader {
        private readonly ILogger _logger;
        private readonly IAssemblyDiscovery _assemblyDiscovery;
        private readonly ModLoaderConfig _modLoaderConfig;
        private readonly UReflectionPointerScanner _uReflectionPointerScanner;

        public ModLoader(ILogger logger, IAssemblyDiscovery assemblyDiscovery, ModLoaderConfig modLoaderConfig, UReflectionPointerScanner uReflectionPointerScanner) {
            _logger = logger;
            _assemblyDiscovery = assemblyDiscovery;
            _modLoaderConfig = modLoaderConfig;
            _uReflectionPointerScanner = uReflectionPointerScanner;
        }

        public void LoadMods() {
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                _logger.Info("Detected Server environment. Loading Server mods only.");
            }

            if (!_modLoaderConfig.EnableMods) {
                _logger.Warning("Mod loading has been disabled in config! Not loading any mods");
                return;
            }

            ScanRuntime();

            _logger.Info("Loading mods");
            var validMods = _assemblyDiscovery.DiscoverValidModAsselblies();
            foreach (var mod in validMods) {
                if (_modLoaderConfig.EnableModWhiteList && !_modLoaderConfig.ModWhiteList.Contains(mod.PalworldModAttribute.ModName)) {
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

                var loadedMod = new LoadedMod(modEntryPoint, mod, _logger);
            }
        }

        internal void ScanRuntime() {
            _logger.Debug("Scanning for reflected functions.");
            _uReflectionPointerScanner.ScanMemoryForUnrealReflectionPointers();
        }
    }
}
