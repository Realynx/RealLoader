using System.Reflection;

using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Extensions {
    public static class BulkPatternScannerExtensions {
        public static IBulkPatternScanner RegisterProperty(this IBulkPatternScanner bulkPatternScanner, PropertyInfo propertyInfo, object instance) {

            var patternAttribute = propertyInfo.GetCustomAttribute<MachineCodePatternAttribute>()
                ?? throw new Exception($"Property: {propertyInfo.Name} does not have a {nameof(MachineCodePatternAttribute)} attribute.");

            bulkPatternScanner.BuildPatternAndAdd(propertyInfo, patternAttribute.Pattern, patternAttribute.PatternType, instance);
            return bulkPatternScanner;
        }

        public static IBulkPatternScanner UpdatePropertyValues(this IBulkPatternScanner bulkTypePatternScanner, IPropertyManager propertyManager) {
            foreach (var member in bulkTypePatternScanner.GetAllRegisteredMembers()) {
                if (member is not PropertyInfo propertyInfo) {
                    continue;
                }

                var byteCodePattern = bulkTypePatternScanner.GetRegisteredByteCode(member);
                var matchedAddress = bulkTypePatternScanner.GetMatchedAddress(byteCodePattern);
                var instance = bulkTypePatternScanner.GetRegisteredTypeInstance(member);

                if (matchedAddress is null) {
                    continue;
                }

                propertyManager.UpdatePropertyValue(byteCodePattern, matchedAddress.Value, propertyInfo, instance);
            }

            return bulkTypePatternScanner;
        }

        public static void BuildPatternAndAdd(this IBulkPatternScanner bulkPatternScanner, MemberInfo memberInfo, string patternString,
            PatternType patternType, object instance = null!) {

            var byteCodePattern = ByteCodePattern.DeriveMask(patternString, patternType);
            bulkPatternScanner.AddPattern(memberInfo, byteCodePattern, instance);
        }
    }
}
