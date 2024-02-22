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
            var detourAttribute = FindDetourAttribute(detourMethod);
            var parentFields = detourMethod.DeclaringType!.GetFields(BindingFlags.Public | BindingFlags.Static);
            var delegateField = parentFields.FirstOrDefault(i => i.Name.Equals($"{detourMethod.Name}_Original", StringComparison.OrdinalIgnoreCase));

            if (detourAttribute is not null && delegateField is { FieldType.IsFunctionPointer: true }) {
                return new ManagedDetourInfo(detourMethod, delegateField, detourAttribute.DetourType);
            }

            _logger.Error($"Detour: '{detourMethod.Name}' Could not find a valid deleagte pointer to assign to detour trampoline!");
            return null;
        }

        public DetourAttribute FindDetourAttribute(MethodInfo detourMethod) {
            var detourAttributes = detourMethod.GetCustomAttributes<DetourAttribute>();
            var patternAttributes = detourMethod.GetCustomAttributes<DetourAttribute>()
                ?? throw new Exception($"Method: {detourMethod.Name} does not have a {nameof(DetourAttribute)} attribute.");

            return FindDetourAttribute(detourAttributes);
        }

        public DetourAttribute FindDetourAttribute(IEnumerable<DetourAttribute> patternAttributes) {
            var windowsDetour = patternAttributes.SingleOrDefault(i => i is WindowsDetourAttribute);
            var linuxDetour = patternAttributes.SingleOrDefault(i => i is LinuxDetourAttribute);
            var detour = patternAttributes.SingleOrDefault(i => i is DetourAttribute and not WindowsDetourAttribute and not LinuxDetourAttribute);

            DetourAttribute? selectedDetourAttribute = null;
            if (linuxDetour is not null && Environment.OSVersion.Platform == PlatformID.Unix) {
                selectedDetourAttribute = linuxDetour;
            }
            else if (windowsDetour is not null && Environment.OSVersion.Platform == PlatformID.Win32NT) {
                selectedDetourAttribute = windowsDetour;
            }
            else if (detour is not null) {
                selectedDetourAttribute = detour;
            }

            if (selectedDetourAttribute is null) {
                throw new Exception("Could not find a valid detour attribute!");
            }

            return selectedDetourAttribute;
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
