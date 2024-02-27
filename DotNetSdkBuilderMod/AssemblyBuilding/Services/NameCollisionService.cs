using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class NameCollisionService : INameCollisionService {
        private readonly ILogger _logger;
        private readonly HashSet<string> _keywords;

        public NameCollisionService(ILogger logger) {
            _logger = logger;
            _keywords = new HashSet<string> {
                "bool",
                "byte",
                "sbyte",
                "short",
                "ushort",
                "int",
                "uint",
                "long",
                "ulong",
                "decimal",
                "float",
                "double",
                "nint",
                "nuint",
                "string",
                "char",
                "namespace",
                "var",
                "class",
                "struct",
                "Dispose",
                "GetType",
            };
        }

        public string GetNonCollidingName(string name, HashSet<string> existingNames) {
            if (IsKeyword(name)) {
                name = $"_{name}";
            }

            var nonCollidingName = name;
            var i = 1;
            while (!existingNames.Add(nonCollidingName)) {
                nonCollidingName = $"{name}_{i}";
                i++;
            }

            return nonCollidingName;
        }

        private bool IsKeyword(string name) {
            return _keywords.Contains(name);
        }
    }
}