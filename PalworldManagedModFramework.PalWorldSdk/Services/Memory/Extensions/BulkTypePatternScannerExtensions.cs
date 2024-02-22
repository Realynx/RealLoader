using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Extensions {
    public static class BulkTypePatternScannerExtensions {
        public static IBulkTypePatternScanner RegisterProperty(this IBulkTypePatternScanner bulkTypePatternScanner, PropertyInfo propertyInfo, object instance) {
            var patternAttribute = propertyInfo.GetCustomAttribute<MachineCodePatternAttribute>()
                ?? throw new Exception($"Property: {propertyInfo.Name} does not have a {nameof(MachineCodePatternAttribute)} attribute.");

            bulkTypePatternScanner.BuildPatternAndAdd(propertyInfo, patternAttribute.Pattern, patternAttribute.PatternType, instance);

            return bulkTypePatternScanner;
        }

        public static IBulkTypePatternScanner RegisterDetour(this IBulkTypePatternScanner bulkTypePatternScanner, MethodInfo methodInfo, IDetourAttributeScanner detourAttributeScanner) {
            var selectedDetourAttribute = detourAttributeScanner.FindDetourAttribute(methodInfo);
            bulkTypePatternScanner.BuildPatternAndAdd(methodInfo, selectedDetourAttribute.Pattern, PatternType.Hook);

            return bulkTypePatternScanner;
        }

        private static void BuildPatternAndAdd(this IBulkTypePatternScanner bulkTypePatternScanner, MemberInfo memberInfo, string patternString, PatternType patternType, object instance = null!) {
            var byteCodePattern = ByteCodePattern.DeriveMask(patternString, patternType);
            bulkTypePatternScanner.AddPattern(memberInfo, byteCodePattern, instance);
        }

        public static IBulkTypePatternScanner UpdatePropertyValues(this IBulkTypePatternScanner bulkTypePatternScanner,
            IPropertyManager propertyManager) {

            foreach (var member in bulkTypePatternScanner.GetAllRegistredMembers()) {
                if (member is not PropertyInfo propertyInfo) {
                    continue;
                }

                var byteCodePattern = bulkTypePatternScanner.GetRegistredByteCode(member);
                var matchedAddress = bulkTypePatternScanner.GetMatchedAddress(byteCodePattern);
                var instance = bulkTypePatternScanner.GetRegistredTypeInstance(member);

                if (matchedAddress is null) {
                    continue;
                }

                propertyManager.UpdatePropertyValue(byteCodePattern, matchedAddress.Value, propertyInfo, instance);
            }

            return bulkTypePatternScanner;
        }
    }
}
