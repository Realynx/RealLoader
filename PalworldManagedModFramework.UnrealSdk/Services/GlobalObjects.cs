using System.Text;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GObjectsStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace PalworldManagedModFramework.UnrealSdk.Services {
    public class GlobalObjects : IGlobalObjects {
        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly unsafe FUObjectArray* _objectPoolAddress;

        public unsafe GlobalObjects(ILogger logger, IEnginePattern enginePattern) {
            _logger = logger;
            _enginePattern = enginePattern;

            _objectPoolAddress = (FUObjectArray*)_enginePattern.PGUObjectArray;
        }

        public unsafe ICollection<UObjectBase> EnumerateObjects() {
            var objectList = new List<UObjectBase>();

            var fixedChunkedArray = _objectPoolAddress->ObjObjects;
            var objects = *fixedChunkedArray.objects;

            for (var x = 0; x < fixedChunkedArray.numElements; x++) {
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

        public unsafe UObjectBase? FindObjects(string name, StringComparison stringComparison = StringComparison.Ordinal) {
            var fixedChunkedArray = _objectPoolAddress->ObjObjects;
            var objects = *fixedChunkedArray.objects;

            for (var x = 0; x < fixedChunkedArray.numElements; x++) {
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
            var fixedChunkedArray = _objectPoolAddress->ObjObjects;
            var objects = *fixedChunkedArray.objects;

            for (var x = 0; x < fixedChunkedArray.numElements; x++) {
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

        public unsafe ICollection<UObjectBase> EnumerateParents() {
            var uniqueParent = new HashSet<UObjectBase>();

            foreach (var uObject in EnumerateObjects()) {
                var parentMostObjectBase = uObject;
                while (parentMostObjectBase.outerPrivate is not null) {
                    parentMostObjectBase = parentMostObjectBase.outerPrivate->baseObjectBaseUtility.baseUObjectBase;
                }

                uniqueParent.Add(parentMostObjectBase);
            }

            return uniqueParent.ToList();
        }

        public unsafe ICollection<UObjectBase> EnumeratePackages() {
            var uniqueParent = new HashSet<UObjectBase>();

            foreach (var uObject in EnumerateEverything()) {
                var objectName = GetNameString(uObject.namePrivate.comparisonIndex);
                if (objectName.StartsWith("/")) {
                    uniqueParent.Add(uObject);
                }
            }

            return uniqueParent.ToList();
        }

        public unsafe ICollection<UObjectBase> EnumerateEverything() {
            var objectPoolAddress = (FUObjectArray*)_enginePattern.PGUObjectArray;

            var rootObjects = EnumerateObjects()
                .ToArray();

            var threadingOptions = new ParallelOptions() {
                MaxDegreeOfParallelism = 25
            };

            var threadDataBlocks = new List<UObjectBase>[rootObjects.Length];
            Parallel.For(0, rootObjects.Length, x => {
                var parentMostObjectBase = rootObjects[x];
                threadDataBlocks[x] = new List<UObjectBase>();

                while (true) {
                    if (parentMostObjectBase.outerPrivate is null) {
                        break;
                    }

                    threadDataBlocks[x].Add(parentMostObjectBase);
                    parentMostObjectBase = parentMostObjectBase.outerPrivate->baseObjectBaseUtility.baseUObjectBase;
                }
            });

            return threadDataBlocks.SelectMany(i => i).ToArray();
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
