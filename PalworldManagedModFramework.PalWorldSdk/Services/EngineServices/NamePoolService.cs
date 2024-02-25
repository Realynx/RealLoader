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
                return Encoding.Unicode.GetString(&nameEntry->stringContents, nameEntry->header.Len);
            }
            else {
                return Encoding.UTF8.GetString(&nameEntry->stringContents, nameEntry->header.Len);
            }
        }

        public unsafe string GetSanitizedNameString(FNameEntryId fnameEntryId) {
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
            const string REPLACEMENT_STRING = "_";
            const char REPLACEMENT_CHAR = '_';

            // non-ordinal string.Replace always allocates a new string, so we check if there is an invalid char first
            var index = str.IndexOf(' ', StringComparison.InvariantCulture);
            if (index is not -1) {
                str = str.Replace(" ", REPLACEMENT_STRING, StringComparison.InvariantCulture);
            }

            index = str.AsSpan().IndexOfAny("-.?\uFFFD");
            if (index is -1) {
                return str;
            }

            return new StringBuilder(str)
                .Replace('-', REPLACEMENT_CHAR)
                .Replace('.', REPLACEMENT_CHAR)
                .Replace('?', REPLACEMENT_CHAR)
                .Replace('\uFFFD', REPLACEMENT_CHAR)
                .ToString();
        }
    }
}
