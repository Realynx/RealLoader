using System.Reflection;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;
using PalworldManagedModFramework.Sdk.Services.Memory.Extensions;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Extensions {
    public static class BulkPatternScannerExtensions {
        private static readonly Dictionary<IBulkPatternScanner, List<DetourRecord>> _detourRecords = new();

        public static IBulkPatternScanner RegisterPatternDetour(this IBulkPatternScanner bulkPatternScanner, MethodInfo methodInfo, string pattern) {
            bulkPatternScanner
                .BuildPatternAndAdd(methodInfo, pattern, PatternType.Detour);

            return bulkPatternScanner;
        }

        public static IBulkPatternScanner PrepareDetours(this IBulkPatternScanner bulkPatternScanner,
            IDetourAttributeService detourAttributeScanner, IDetourManager detourManager) {

            foreach (var member in bulkPatternScanner.GetAllRegisteredMembers()) {
                if (member is not MethodInfo methodInfo) {
                    continue;
                }

                var byteCodePattern = bulkPatternScanner.GetRegisteredByteCode(member);
                var matchedAddress = bulkPatternScanner.GetMatchedAddress(byteCodePattern);

                var detourInfo = detourAttributeScanner.GetManagedDetourInfo(methodInfo);
                if (detourInfo is null || matchedAddress is null) {
                    continue;
                }

                var detourRecord = detourManager.PrepareDetour(detourInfo, matchedAddress.Value);

                ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(_detourRecords,
                    bulkPatternScanner, out var previouslyExisted);

                if (!previouslyExisted) {
                    value = new List<DetourRecord>();
                }

                value!.Add(detourRecord);
            }

            return bulkPatternScanner;
        }

        public static IBulkPatternScanner InstallDetours(this IBulkPatternScanner bulkPatternScanner, IDetourManager detourManager) {
            if (_detourRecords.TryGetValue(bulkPatternScanner, out var detourRecords)) {
                var toRemove = new List<DetourRecord>();

                foreach (var detour in detourRecords) {
                    if (detourManager.InstallDetour(detour)) {
                        toRemove.Add(detour);
                    }
                }

                foreach (var detour in toRemove) {
                    detourRecords.Remove(detour);
                }
            }

            return bulkPatternScanner;
        }
    }
}
