using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public class DetourAttributeScanner : IDetourAttributeScanner {
        private readonly ILogger _logger;

        public DetourAttributeScanner(ILogger logger) {
            _logger = logger;
        }

        public ManagedDetourInfo? FindDetourInformation(MethodInfo detourMethod) {
            var detourAttribute = detourMethod.GetCustomAttribute<DetourAttribute>();
            var parentFields = detourMethod.DeclaringType!.GetFields(BindingFlags.Public | BindingFlags.Static);
            var delegateField = parentFields.FirstOrDefault(i => i.Name.Equals($"{detourMethod.Name}_Original", StringComparison.OrdinalIgnoreCase));

            if (detourAttribute is not null && delegateField is { FieldType.IsFunctionPointer: true }) {
                return new ManagedDetourInfo(detourMethod, delegateField, detourAttribute.DetourType);
            }

            _logger.Error($"Detour: '{detourMethod.Name}' Could not find a valid deleagte pointer to assign to detour trampoline!");
            return null;
        }

        public ManagedDetourInfo[] FindAllDetourInfos(IEnumerable<Assembly> assemblies) {
            var foundDetours = new HashSet<ManagedDetourInfo>();

            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();

                var allAttributedMethods = new List<MethodInfo>();
                foreach (var type in types) {
                    allAttributedMethods.AddRange(SearchAttributes(type));
                }

                foreach (var methodInfo in allAttributedMethods) {
                    var detourInfo = FindDetourInformation(methodInfo);
                    if (detourInfo is not { }) {
                        continue;
                    }

                    foundDetours.Add(detourInfo);
                }
            }

            return foundDetours.ToArray();
        }

        private ICollection<MethodInfo> SearchAttributes(Type type) {
            var foundAttributeMethods = new List<MethodInfo>();

            if (type.BaseType is not null) {
                foundAttributeMethods.AddRange(SearchAttributes(type.BaseType));
            }

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods) {
                var customAttribute = method.GetCustomAttribute<DetourAttribute>();
                if (customAttribute is not null) {
                    foundAttributeMethods.Add(method);
                }
            }

            return foundAttributeMethods;
        }
    }
}
