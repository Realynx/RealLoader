using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GObjectsStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace PalworldManagedModFramework.UnrealSdk.Services {
    public class GlobalObjects : IGlobalObjects {
        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly INamePoolService _namePoolService;
        private readonly unsafe FUObjectArray* _objectPoolAddress;

        public unsafe GlobalObjects(ILogger logger, IEnginePattern enginePattern, INamePoolService namePoolService) {
            _logger = logger;
            _enginePattern = enginePattern;
            _namePoolService = namePoolService;
            _objectPoolAddress = (FUObjectArray*)_enginePattern.PGUObjectArray;
        }

        public unsafe UObjectBase*[] EnumerateObjects() {
            var fixedChunkedArray = _objectPoolAddress->ObjObjects;
            var objects = *fixedChunkedArray.objects;

            var objectList = new UObjectBase*[fixedChunkedArray.numElements];
            for (var x = 0; x < fixedChunkedArray.numElements; x++) {
                var currentItem = objects + x;

                if (currentItem is not null) {
                    if (currentItem->uObject is not null) {
                        var pObjectBase = currentItem->uObject;
                        objectList[x] = pObjectBase;
                    }
                }
            }

            return objectList;
        }

        public unsafe UObjectBase* FindObjects(string name, StringComparison stringComparison = StringComparison.Ordinal) {
            var fixedChunkedArray = _objectPoolAddress->ObjObjects;
            var objects = *fixedChunkedArray.objects;

            for (var x = 0; x < fixedChunkedArray.numElements; x++) {
                var currentItem = objects + x;

                if (currentItem is not null) {
                    if (currentItem->uObject is not null) {
                        var pUobject = currentItem->uObject;

                        var nameEntryId = pUobject->namePrivate.comparisonIndex;
                        var objectName = _namePoolService.GetNameString(nameEntryId);
                        if (objectName.Equals(name, stringComparison)) {
                            return pUobject;
                        }
                    }
                }
            }

            return null;
        }

        public unsafe UObjectBase*[] EnumerateEverything() {
            // var objectPoolAddress = (FUObjectArray*)_enginePattern.PGUObjectArray;
            var rootObjects = EnumerateObjects();
            var pointers = new List<nint>();

            foreach (var pObject in rootObjects) {
                for (UObjectBase* pCurrentObject = pObject; pCurrentObject is not null; pCurrentObject = (UObjectBase*)pCurrentObject->outerPrivate) {
                    pointers.Add((nint)pCurrentObject);
                }
            }

            var objectArray = new UObjectBase*[pointers.Count];
            for (var x = 0; x < objectArray.Length; x++) {
                objectArray[x] = (UObjectBase*)pointers[x];
            }

            return objectArray;
        }
    }
}
