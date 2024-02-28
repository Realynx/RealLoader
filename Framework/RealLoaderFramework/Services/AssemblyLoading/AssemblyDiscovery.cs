using System.Reflection;
using System.Runtime.InteropServices;

using RealLoaderFramework.Models;
using RealLoaderFramework.Models.Config;
using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Services.AssemblyLoading.Interfaces;

namespace RealLoaderFramework.Services.AssemblyLoading {
    internal class AssemblyDiscovery : IAssemblyDiscovery {

        private readonly ILogger _logger;
        private readonly ModLoaderConfig _modLoaderConfig;

        public AssemblyDiscovery(ILogger logger, ModLoaderConfig modLoaderConfig) {
            _logger = logger;
            _modLoaderConfig = modLoaderConfig;
        }

        public IEnumerable<ClrMod> DiscoverValidModAsselblies() {
            var modFolderPath = Path.GetFullPath(_modLoaderConfig.ModFolder);
            if (!Directory.Exists(modFolderPath)) {
                _logger.Error($"The mod's folder does not exist. '{modFolderPath}'");
                yield break;
            }

            var modFolders = Directory.GetDirectories(modFolderPath);
            foreach (var modFolder in modFolders) {
                var runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll").ToList();
                var dllFiles = Directory.GetFiles(modFolder, "*.dll");
                runtimeAssemblies.AddRange(dllFiles);

                foreach (var dllAssembly in dllFiles) {
                    var fileName = Path.GetFileName(dllAssembly);
                    _logger.Debug($"[{fileName}] Loading Assembly Context.");

                    Assembly assembly;
                    try {
                        assembly = Assembly.LoadFrom(dllAssembly);
                    }
                    catch (FileLoadException) {
                        _logger.Error($"[{fileName}] Failed to reflect/load assembly.");
                        continue;
                    }

                    var modAttributeParent = assembly.GetTypes().FirstOrDefault(i => i.GetCustomAttribute<RealLoaderModAttribute>() is not null);
                    if (modAttributeParent == null) {
                        _logger.Debug($"[{fileName}] No {nameof(RealLoaderModAttribute)} attribute found.");
                        continue;
                    }

                    var modAttribute = modAttributeParent.GetCustomAttribute<RealLoaderModAttribute>();
                    yield return new ClrMod() {
                        AssemblyPath = dllAssembly,
                        ModAttribute = modAttribute,
                        Assembly = assembly
                    };
                }
            }
        }
    }
}
