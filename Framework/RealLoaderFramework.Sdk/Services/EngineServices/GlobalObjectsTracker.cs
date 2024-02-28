using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;
using RealLoaderFramework.Sdk.Services.Detour.Models;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;

namespace RealLoaderFramework.Sdk.Services.EngineServices {
    public class GlobalObjectsTracker : IGlobalObjectsTracker {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;
        private readonly IUnrealEventRegistrationService _unrealEventRegistrationService;
        private static GlobalObjectsTracker? _thisInstance;

        private readonly HashSet<nint> _loadedObjects = new();
        private readonly HashSet<nint> _markedObjects = new();

        private bool _synchronized = false;
        public GlobalObjectsTracker(ILogger logger, IGlobalObjects globalObjects, IUnrealEventRegistrationService unrealEventRegistrationService) {
            _logger = logger;
            _globalObjects = globalObjects;
            _unrealEventRegistrationService = unrealEventRegistrationService;
            _thisInstance = this;

            _unrealEventRegistrationService
                .FindAndRegisterEvents<IGlobalObjectsTracker>(this);
        }


        /// <summary>
        /// Fires for client.
        /// </summary>
        /// <param name="unrealEvent"></param>
        [EngineEvent("^WBP_TItle_C::OnInitialized")]
        public unsafe void OnInitialized(UnrealEvent _) {
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
        public unsafe void OnEndedWorldAutoSave(UnrealEvent _) {
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
