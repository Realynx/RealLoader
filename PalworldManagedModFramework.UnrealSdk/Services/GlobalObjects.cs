using System.Text;

using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

using static PalworldManagedModFramework.UnrealSdk.Services.Structs.GNamesStructs;
using static PalworldManagedModFramework.UnrealSdk.Services.Structs.GObjectsStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services {
    public class GlobalObjects : IGlobalObjects {
        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;

        public GlobalObjects(ILogger logger, IEnginePattern enginePattern) {
            _logger = logger;
            _enginePattern = enginePattern;
        }

        public unsafe UObjectBase? FindObjects(string name, StringComparison stringComparison = StringComparison.Ordinal) {
            var objectPoolAddress = (FUObjectArray*)_enginePattern.PGUObjectArray;
            var objects = *objectPoolAddress->objects;
            for (var x = 0; x < objectPoolAddress->numElements; x++) {
                var currentItem = objects + x;

                if (currentItem is not null) {
                    if (currentItem->uObject is not null) {
                        var uobject = currentItem->uObject;

                        var nameEntryId = uobject->namePrivate.comparisonIndex;
                        var objectName = GetNameString(nameEntryId);
                        if (objectName.Equals(name, stringComparison)) {
                            return *uobject;
                        }
                    }
                }
            }

            return null;
        }

        public unsafe IDictionary<string, ICollection<UObjectBase>> EnumerateNamedObjects() {
            var objectDictionary = new Dictionary<string, ICollection<UObjectBase>>();

            var objectPoolAddress = (FUObjectArray*)_enginePattern.PGUObjectArray;
            var objects = *objectPoolAddress->objects;
            for (var x = 0; x < objectPoolAddress->numElements; x++) {
                var currentItem = objects + x;

                if (currentItem is not null) {
                    if (currentItem->uObject is not null) {
                        var objectBase = *currentItem->uObject;
                        var objectName = GetNameString(objectBase.namePrivate.comparisonIndex);

                        if (!objectDictionary.TryAdd(objectName, new List<UObjectBase>() { objectBase })) {
                            objectDictionary[objectName].Add(objectBase);
                        }

                    }
                }
            }

            return objectDictionary;
        }

        public unsafe ICollection<UObjectBase> EnumerateObjects() {
            var objectList = new List<UObjectBase>();

            var objectPoolAddress = (FUObjectArray*)_enginePattern.PGUObjectArray;
            var objects = *objectPoolAddress->objects;
            for (var x = 0; x < objectPoolAddress->numElements; x++) {
                var currentItem = objects + x;

                if (currentItem is not null) {
                    if (currentItem->uObject is not null) {
                        var objectBase = *currentItem->uObject;
                        objectList.Add(objectBase);
                    }
                }
            }

            return objectList;
        }

        public unsafe FNameEntry* GetName(FNameEntryId fnameEntryId) {
            var namePool = _enginePattern.PNamePoolData;
            var nameBlockOffset = fnameEntryId.lowerOrderValue * 2;

            // Windows lock is 0x8 bytes
            // Linux lock is 0x38 bytes

            var offsetSize = 0x10;
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                offsetSize = 0x40;
            }

            var namePointerBlock = (nint*)(namePool + offsetSize + (fnameEntryId.higherOrderValue * 8));
            return (FNameEntry*)(*namePointerBlock + nameBlockOffset);
        }

        public unsafe string GetNameString(FNameEntryId fnameEntryId) {
            var nameEntry = GetName(fnameEntryId);
            var stringValue = Encoding.UTF8.GetString(&nameEntry->StringPointer, nameEntry->header.Len);

            return stringValue;
        }
    }
}
