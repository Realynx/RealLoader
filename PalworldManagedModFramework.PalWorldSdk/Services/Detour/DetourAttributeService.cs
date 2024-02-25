using System.Reflection;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public class DetourAttributeService : IDetourAttributeService {
        private readonly ILogger _logger;

        public DetourAttributeService(ILogger logger) {
            _logger = logger;
        }

        public ManagedDetourInfo? GetManagedDetourInfo(MethodInfo detourMethod) {
            var detourAttribute = FindDetourAttribute(detourMethod);

            var parentFields = detourMethod.DeclaringType!.GetFields(BindingFlags.Public | BindingFlags.Static);
            var delegateField = parentFields.FirstOrDefault(i => i.Name.Equals($"{detourMethod.Name}_Original", StringComparison.OrdinalIgnoreCase));

            if (delegateField is { FieldType.IsFunctionPointer: true }) {
                return new ManagedDetourInfo(detourMethod, delegateField, detourAttribute.DetourType);
            }

            _logger.Error($"Detour: '{detourMethod.Name}' Could not find a valid delegate pointer to assign to detour trampoline!");
            return null;
        }

        public DetourAttribute? FindDetourAttribute(MethodInfo detourMethod) {
            var detourAttributes = detourMethod.GetCustomAttributes<DetourAttribute>()
                ?? throw new Exception($"Method: {detourMethod.Name} does not have a {nameof(DetourAttribute)} attribute.");

            return FindPatternDetourAttribute(detourAttributes);
        }

        public DetourAttribute? FindPatternDetourAttribute(IEnumerable<DetourAttribute> patternAttributes) {
            var linuxDetours = patternAttributes.Where(i => i is LinuxDetourAttribute);
            var windowsDetours = patternAttributes.Where(i => i is WindowsDetourAttribute);
            var detours = patternAttributes.Where(i => i is not null and not WindowsDetourAttribute and not LinuxDetourAttribute);

            DetourAttribute? selectedDetourAttribute = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && linuxDetours.SingleOrDefault() is { } linuxDetour) {
                selectedDetourAttribute = linuxDetour;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && windowsDetours.SingleOrDefault() is { } windowsDetour) {
                selectedDetourAttribute = windowsDetour;
            }
            else if (detours.SingleOrDefault() is { } detour) {
                selectedDetourAttribute = detour;
            }

            return selectedDetourAttribute;
        }
    }
}
