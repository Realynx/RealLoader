using PalworldManagedModFramework.Models;
using PalworldManagedModFramework.Models.Config;
using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;
using PalworldManagedModFramework.Services.Interfaces;
using PalworldManagedModFramework.Services.SandboxDI.Interfaces;

namespace PalworldManagedModFramework.Services.AssemblyLoading {
    internal class ModLoader : IModLoader {
        private readonly ILogger _logger;
        private readonly IAssemblyDiscovery _assemblyDiscovery;
        private readonly ModLoaderConfig _modLoaderConfig;
        private readonly IRuntimeInstaller _runtimeInstaller;
        private readonly ISandboxDIService _sandboxDIService;
        private readonly HashSet<LoadedMod> _loadedMods = [];

        public ModLoader(ILogger logger, IAssemblyDiscovery assemblyDiscovery, ModLoaderConfig modLoaderConfig,
            IRuntimeInstaller runtimeInstaller, ISandboxDIService sandboxDIService) {

            _logger = logger;
            _assemblyDiscovery = assemblyDiscovery;
            _modLoaderConfig = modLoaderConfig;
            _runtimeInstaller = runtimeInstaller;
            _sandboxDIService = sandboxDIService;
        }

        public void LoadMods() {
            var loadableMods = GetCurrentModFlags();

            if (!_modLoaderConfig.EnableMods) {
                _logger.Warning("Mod loading has been disabled in config! Not loading any mods");
                return;
            }

            _logger.Debug("Scanning for reflected functions.");
            _runtimeInstaller.ScanAndInstallRuntime();

            _logger.Info("Loading mods");
            var validMods = _assemblyDiscovery.DiscoverValidModAsselblies();

            LoadMods(loadableMods, validMods);
        }

        private void LoadMods(PalworldModType loadableMods, IEnumerable<ClrMod> validMods) {
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

                var modEntryPoint = GetEntryPoint(mod);
                if (modEntryPoint is null) {
                    _logger.Error($"[{mod.PalworldModAttribute.ModName}] No valid mod entry point was found! " +
                        $"You should contact ({mod.PalworldModAttribute.Author}) {mod.PalworldModAttribute.DiscordAlias} in discord.");

                    continue;
                }

                var loadedMod = new LoadedMod(modEntryPoint, mod, _logger, _sandboxDIService);
                var registered = _loadedMods.Add(loadedMod);

                if (!registered) {
                    _logger.Error($"Attempted to load mod twice! Disposing second instance.");
                    loadedMod.Unload();
                }
            }
        }

        private static Type? GetEntryPoint(ClrMod mod) {
            var modTypes = mod.Assembly.GetTypes();
            var modEntryPoint = modTypes.FirstOrDefault(i => i.GetInterface(nameof(IPalworldMod)) is not null);
            return modEntryPoint;
        }

        private PalworldModType GetCurrentModFlags() {
            var loadableMods = PalworldModType.Universal;
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                loadableMods |= PalworldModType.Server;

                _logger.Info("Detected Server environment. Loading Server and Universal mods only.");
            }
            else {
                loadableMods |= PalworldModType.Client;
            }

            return loadableMods;
        }
    }
}
