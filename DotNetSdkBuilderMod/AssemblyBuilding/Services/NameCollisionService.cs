using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class NameCollisionService : INameCollisionService {
        private readonly ILogger _logger;

        public NameCollisionService(ILogger logger) {
            _logger = logger;
        }

        public string GetNonCollidingName(string name, HashSet<string> existingNames) {
            var nonCollidingName = name;
            var i = 1;
            while (!existingNames.Add(nonCollidingName)) {
                nonCollidingName = $"{name}_{i}";
                i++;
            }

            return nonCollidingName;
        }
    }
}