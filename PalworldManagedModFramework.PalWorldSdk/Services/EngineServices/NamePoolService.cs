using System.Text;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.GNameStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
    public class NamePoolService : INamePoolService {
        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;

        public NamePoolService(ILogger logger, IEnginePattern enginePattern) {
            _logger = logger;
            _enginePattern = enginePattern;
        }

        public unsafe FNameEntry* GetName(FNameEntryId fnameEntryId) {
            var namePool = _enginePattern.PNamePoolData;

            // Windows lock object is 0x8 bytes
            // Linux lock object is 0x38 bytes

            var offsetSize = 0x10;
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                offsetSize = 0x40;
            }

            var nameBlockOffset = fnameEntryId.LowerOrderValue * 2;
            var namePointerBlock = (nint*)(namePool + offsetSize + fnameEntryId.HigherOrderValue * 8);

            return (FNameEntry*)(*namePointerBlock + nameBlockOffset);
        }

        public unsafe string GetNameString(FNameEntryId fnameEntryId) {
            var nameEntry = GetName(fnameEntryId);

            if (nameEntry->header.BIsWide) {
                var decoded = Encoding.Unicode.GetString(&nameEntry->stringContents, nameEntry->header.Len);
                return RemoveInvalidChars(decoded);
            }
            else {
                var decoded = Encoding.UTF8.GetString(&nameEntry->stringContents, nameEntry->header.Len);
                return RemoveInvalidChars(decoded);
            }
        }

        private static string RemoveInvalidChars(string str) {
            return str
                .Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase)
                .Replace("-", "_")
                .Replace(".", "_")
                .Replace("?", "_")
                .Replace("\uFFFD", "_");
        }
    }
}
