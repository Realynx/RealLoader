using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;

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
                "fixed",
                "Dispose",
                "GetType",
            };
        }

        public string GetNonCollidingName(string name, HashSet<string> existingNames) {
            if (IsDigitOrKeyword(name)) {
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

        public string GetNonCollidingName(string name) {
            if (IsDigitOrKeyword(name)) {
                return $"_{name}";
            }

            return name;
        }

        private bool IsDigitOrKeyword(string name) {
            return char.IsDigit(name[0]) || _keywords.Contains(name);
        }
    }
}