using System.Reflection;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Extensions {
    public static class DetourBulkScannerExtensions {
        private static readonly Dictionary<IBulkTypePatternScanner, List<DetourRecord>> _detourRecords = new();

        public static IBulkTypePatternScanner PrepareDetours(this IBulkTypePatternScanner bulkTypePatternScanner,
            IDetourAttributeScanner detourAttributeScanner, IDetourManager detourManager) {

            foreach (var member in bulkTypePatternScanner.GetAllRegistredMembers()) {
                if (member is not MethodInfo methodInfo) {
                    continue;
                }

                var byteCodePattern = bulkTypePatternScanner.GetRegistredByteCode(member);
                var matchedAddress = bulkTypePatternScanner.GetMatchedAddress(byteCodePattern);

                var detourInfo = detourAttributeScanner.FindDetourInformation(methodInfo);
                if (detourInfo is null || matchedAddress is null) {
                    continue;
                }

                var detourRecord = detourManager.PrepareDetour(detourInfo, matchedAddress.Value);

                ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(_detourRecords,
                    bulkTypePatternScanner, out var previouslyExisted);

                if (!previouslyExisted) {
                    value = new List<DetourRecord>();
                }

                value!.Add(detourRecord);
            }

            return bulkTypePatternScanner;
        }

        public static IBulkTypePatternScanner InstallDetours(this IBulkTypePatternScanner bulkTypePatternScanner, IDetourManager detourManager) {
            if (_detourRecords.TryGetValue(bulkTypePatternScanner, out var detourRecords)) {
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

            return bulkTypePatternScanner;
        }
    }
}
