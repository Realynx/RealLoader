using System.Linq;
using System.Text;

using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GobjectsStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

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

        public unsafe ICollection<UObjectBase> EnumerateRootObjects() {
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

        public unsafe ICollection<UObjectBase> EnumerateParents() {
            var uniqueParent = new HashSet<UObjectBase>();

            foreach (var uObject in EnumerateRootObjects()) {
                var parentMostObjectBase = uObject;
                while (parentMostObjectBase.outerPrivate is not null) {
                    parentMostObjectBase = parentMostObjectBase.outerPrivate->baseObjectBaseUtility.baseUObjectBase;
                }

                uniqueParent.Add(parentMostObjectBase);
            }

            return uniqueParent.ToList();
        }

        public unsafe ICollection<UObjectBase> EnumerateEverything() {
            var uniqueParent = new HashSet<UObjectBase>();
            var rootObjects = EnumerateRootObjects();

            var threadingOptions = new ParallelOptions() {
                MaxDegreeOfParallelism = 200
            };

            Parallel.ForEach(rootObjects, threadingOptions, (uObject) => {
                var parentMostObjectBase = uObject;

                while (true) {
                    if (parentMostObjectBase.outerPrivate is null) {
                        break;
                    }

                    // HashSet.Contains is O(1) operation, we can check every iteration without performance hit.
                    if (!uniqueParent.Contains(uObject)) {
                        lock (uniqueParent) {
                            uniqueParent.Add(parentMostObjectBase);
                        }
                    }
                    parentMostObjectBase = parentMostObjectBase.outerPrivate->baseObjectBaseUtility.baseUObjectBase;
                }
            });

            return uniqueParent.ToList();
        }

        public unsafe FNameEntry* GetName(FNameEntryId fnameEntryId) {
            var namePool = _enginePattern.PNamePoolData;
            var nameBlockOffset = fnameEntryId.lowerOrderValue * 2;

            // Windows lock object is 0x8 bytes
            // Linux lock object is 0x38 bytes

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
