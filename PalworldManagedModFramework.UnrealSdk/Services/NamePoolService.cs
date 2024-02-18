using System.Text;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services {
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
            var namePointerBlock = (nint*)(namePool + offsetSize + (fnameEntryId.HigherOrderValue * 8));

            return (FNameEntry*)(*namePointerBlock + nameBlockOffset);
        }

        public unsafe string GetNameString(FNameEntryId fnameEntryId) {
            var nameEntry = GetName(fnameEntryId);

            if (nameEntry->header.BIsWide) {
                return Encoding.Unicode.GetString(&nameEntry->stringContents, nameEntry->header.Len).Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);
            }
            else {
                return Encoding.UTF8.GetString(&nameEntry->stringContents, nameEntry->header.Len).Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
