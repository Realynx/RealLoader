using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Detour;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
    public class GlobalObjectsTracker : IGlobalObjectsTracker {
        private readonly ILogger _logger;

        private static GlobalObjectsTracker? _thisInstance;
        private readonly HashSet<nint> _loadedObjects = new();
        private readonly HashSet<nint> _markedObjects = new();

        public GlobalObjectsTracker(ILogger logger) {
            _logger = logger;
            _thisInstance = this;
        }

        public nint[] GetLoadedObjects() {
            return _loadedObjects.ToArray();
        }

        public nint[] GetMarkedObjects() {
            return _markedObjects.ToArray();
        }

        public unsafe bool IsObjectDestroyed(nint uObjectAddress) {
            return !(_loadedObjects.Contains(uObjectAddress) || _markedObjects.Contains(uObjectAddress));
        }

        public unsafe bool IsObjectMarkedForCollection(nint uObjectAddress) {
            return _markedObjects.Contains(uObjectAddress);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ObjectLoaded(nint pObject) {
            _loadedObjects.Add(pObject);

            SetConsoleTitileObjCount();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ObjectDestroying(nint pObject) {
            if (_loadedObjects.Remove(pObject)) {
                _markedObjects.Add(pObject);
            }

            SetConsoleTitileObjCount();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ObjectDestroyed(nint pObject) {

            // Consider the Object as already freed from memory at this point the nint pointer is ID only.
            var removed = _markedObjects.Remove(pObject) || _loadedObjects.Remove(pObject);
            if (!removed) {
                _logger.Error($"Could not remove untracked object: 0x{pObject:X}");
            }

            SetConsoleTitileObjCount();
        }

        private void SetConsoleTitileObjCount() {
            Console.Title = $"Active Objects: {_loadedObjects.Count}, GC Marked Objects: {_markedObjects.Count}";
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectPostInitProperties_Original;
        [Detour("48 83 ? ? 48 ? ? 48 C7 44 24 20 00 00 ? ? 48 8B ? ? 45 ? ? 41 ? ?", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectPostInitProperties(UObject* instance) {
            UObjectPostInitProperties_Original(instance);

            _thisInstance?.ObjectLoaded((nint)instance);
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectBeginDestroy_Original;
        [Detour("40 ? 48 83 ? ? 8B ? ? 48 ? ? C1 ? ? ? ? 75 ? 48 8B ? ? 48 8D 54", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectBeginDestroy(UObject* instance) {
            UObjectBeginDestroy_Original(instance);

            _thisInstance?.ObjectDestroying((nint)instance);
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectFinishDestroy_Original;
        [Detour("40 ? 48 83 ? ? F6 41 ? ? 48 ? ? 75 ? 48 8B ? ? 48 8D 54 ? ? 48 8D 4C", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectFinishDestroy(UObject* instance) {
            _thisInstance?.ObjectDestroyed((nint)instance);

            UObjectFinishDestroy_Original(instance);
        }
    }
}
