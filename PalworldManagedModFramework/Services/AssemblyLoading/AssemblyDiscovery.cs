using System.Reflection;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Models;
using PalworldManagedModFramework.Models.Config;
using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;

namespace PalworldManagedModFramework.Services.AssemblyLoading {
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

                //var resolver = new PathAssemblyResolver(runtimeAssemblies);

                //using var metadataLoadContext = new MetadataLoadContext(resolver);
                foreach (var dllAssembly in dllFiles) {
                    var fileName = Path.GetFileName(dllAssembly);
                    _logger.Debug($"[{fileName}] Loading Assembly Context.");

                    //var assembly = metadataLoadContext.LoadFromAssemblyPath(dllAssembly);
                    Assembly assembly;
                    try {
                        assembly = Assembly.LoadFrom(dllAssembly);
                    }
                    catch (FileLoadException) {
                        _logger.Error($"[{fileName}] Failed to reflect/load assembly.");
                        continue;
                    }

                    var modAttributeParent = assembly.GetTypes().FirstOrDefault(i => i.GetCustomAttribute<PalworldModAttribute>() is not null);
                    if (modAttributeParent == null) {
                        _logger.Debug($"[{fileName}] No {nameof(PalworldModAttribute)} attribute found.");
                        continue;
                    }

                    var modAttribute = modAttributeParent.GetCustomAttribute<PalworldModAttribute>();
                    yield return new ClrMod() {
                        AssemblyPath = dllAssembly,
                        PalworldModAttribute = modAttribute,
                        Assembly = assembly
                    };
                }
            }

        }
    }
}
