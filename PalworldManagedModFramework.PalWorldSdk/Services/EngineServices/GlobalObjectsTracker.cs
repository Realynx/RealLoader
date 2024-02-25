using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.UnrealHook.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
    public class GlobalObjectsTracker : IGlobalObjectsTracker {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;
        private readonly IUnrealHookManager _unrealHookManager;

        private static GlobalObjectsTracker? _thisInstance;

        private readonly HashSet<nint> _loadedObjects = new();
        private readonly HashSet<nint> _markedObjects = new();

        private bool _synchronized = false;
        public GlobalObjectsTracker(ILogger logger, IGlobalObjects globalObjects, IUnrealHookManager unrealHookManager) {
            _logger = logger;
            _globalObjects = globalObjects;
            _unrealHookManager = unrealHookManager;
            _thisInstance = this;

            _unrealHookManager
                .RegisterUnrealEvent(GetType().GetMethod(nameof(OnInitialized))!, this)
                .RegisterUnrealEvent(GetType().GetMethod(nameof(OnEndedWorldAutoSave))!, this);
        }


        /// <summary>
        /// Fires for client.
        /// </summary>
        /// <param name="unrealEvent"></param>
        [EngineEvent("^WBP_TItle_C::OnInitialized")]
        public unsafe void OnInitialized(UnrealEvent unrealEvent) {
            _synchronized = true;
            if (_synchronized) {
                return;
            }

            SynchroniseObjectPool();
            _logger.Debug("Synchronized Object Pool");
        }

        /// <summary>
        /// Fires for server.
        /// </summary>
        /// <param name="unrealEvent"></param>
        [EngineEvent("^PalStaticLogCollector::OnEndedWorldAutoSave")]
        public unsafe void OnEndedWorldAutoSave(UnrealEvent unrealEvent) {
            _synchronized = true;
            if (_synchronized) {
                return;
            }

            SynchroniseObjectPool();
            _logger.Debug("Synchronized Object Pool");
        }

        public unsafe void SynchroniseObjectPool() {
            var currentObjects = _globalObjects.EnumerateObjects();

            foreach (var pObject in currentObjects) {
                if (pObject is null) {
                    continue;
                }

                _loadedObjects.Add((nint)pObject);
            }
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

        [MethodImpl(MethodImplOptions.Synchronized | MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private void ObjectLoaded(nint pObject) {
            _loadedObjects.Add(pObject);

            SetConsoleTitleObjCount();
        }

        [MethodImpl(MethodImplOptions.Synchronized | MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private void ObjectDestroying(nint pObject) {
            if (_loadedObjects.Remove(pObject)) {
                _markedObjects.Add(pObject);
            }

            SetConsoleTitleObjCount();
        }

        [MethodImpl(MethodImplOptions.Synchronized | MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private void ObjectDestroyed(nint pObject) {
            // Consider the Object as already freed from memory at this point the nint pointer is ID only.
            var removed = _markedObjects.Remove(pObject) || _loadedObjects.Remove(pObject);
            //if (!removed) {
            //}

            SetConsoleTitleObjCount();
        }

        private void SetConsoleTitleObjCount() {
            Console.Title = $"Active Objects: {_loadedObjects.Count}, GC Marked Objects: {_markedObjects.Count}";
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectPostInitProperties_Original;
        [EngineDetour(EngineFunction.UObject_PostInitProperties, DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectPostInitProperties(UObject* instance) {
            UObjectPostInitProperties_Original(instance);

            _thisInstance?.ObjectLoaded((nint)instance);
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectBeginDestroy_Original;
        [EngineDetour(EngineFunction.UObject_BeginDestroy, DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectBeginDestroy(UObject* instance) {
            UObjectBeginDestroy_Original(instance);

            _thisInstance?.ObjectDestroying((nint)instance);
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectFinishDestroy_Original;
        [EngineDetour(EngineFunction.UObject_FinishDestroy, DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectFinishDestroy(UObject* instance) {
            _thisInstance?.ObjectDestroyed((nint)instance);

            UObjectFinishDestroy_Original(instance);
        }
    }
}
