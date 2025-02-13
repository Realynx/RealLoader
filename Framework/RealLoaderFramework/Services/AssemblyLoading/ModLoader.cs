using RealLoaderFramework.Models;
using RealLoaderFramework.Models.Config;
using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Interfaces;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;
using RealLoaderFramework.Services.AssemblyLoading.Interfaces;
using RealLoaderFramework.Services.Interfaces;
using RealLoaderFramework.Services.SandboxDI.Interfaces;

namespace RealLoaderFramework.Services.AssemblyLoading {
    internal class ModLoader : IModLoader {
        private readonly ILogger _logger;
        private readonly IAssemblyDiscovery _assemblyDiscovery;
        private readonly ModLoaderConfig _modLoaderConfig;
        private readonly IRuntimeInstaller _runtimeInstaller;
        private readonly ISandboxDIService _sandboxDIService;
        private readonly IGlobalObjectsTracker _globalObjectsTracker;
        private readonly HashSet<LoadedMod> _loadedMods = [];

        public ModLoader(ILogger logger, IAssemblyDiscovery assemblyDiscovery, ModLoaderConfig modLoaderConfig,
            IRuntimeInstaller runtimeInstaller, ISandboxDIService sandboxDIService, IGlobalObjectsTracker globalObjectsTracker) {
            _logger = logger;
            _assemblyDiscovery = assemblyDiscovery;
            _modLoaderConfig = modLoaderConfig;
            _runtimeInstaller = runtimeInstaller;
            _sandboxDIService = sandboxDIService;
            _globalObjectsTracker = globalObjectsTracker;
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

        private void LoadMods(RealLoaderModType loadableMods, IEnumerable<ClrMod> validMods) {
            foreach (var mod in validMods) {
                if (_modLoaderConfig.EnableModWhiteList &&
                    !_modLoaderConfig.ModWhiteList.Contains(mod.ModAttribute.ModName)) {
                    continue;
                }

                if (!loadableMods.HasFlag(mod.ModAttribute.ModType)) {
                    _logger.Warning($"Skipping incompatible mod: [{mod.ModAttribute.ModName}]");
                    continue;
                }

                _logger.Info($"[Loading] Mod: {mod.ModAttribute.ModName}, Author(s): {string.Join(", ", mod.ModAttribute.Authors)}");

                var modEntryPoint = GetEntryPoint(mod);
                if (modEntryPoint is null) {
                    _logger.Error($"[{mod.ModAttribute.ModName}] No valid mod entry point was found! " +
                        $"You should contact ({mod.ModAttribute.Authors}) {mod.ModAttribute.ContactInfo} in discord.");

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
            var modEntryPoint = modTypes.FirstOrDefault(i => i.GetInterface(nameof(IRealLoaderMod)) is not null);
            return modEntryPoint;
        }

        private RealLoaderModType GetCurrentModFlags() {
            var loadableMods = RealLoaderModType.Universal;
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                loadableMods |= RealLoaderModType.Server;

                _logger.Info("Detected Server environment. Loading Server and Universal mods only.");
            }
            else {
                loadableMods |= RealLoaderModType.Client;
            }

            return loadableMods;
        }
    }
}
