using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.FunctionServices;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory;
using PalworldManagedModFramework.Sdk.Services.Memory.Extensions;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class PatternScanner {
        private static ILogger _loggerStatic;
        private static INamePoolService _namePoolServiceStatic;

        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly IUObjectFuncs _uObjectFuncs;
        private readonly IBulkTypePatternScanner _bulkTypePatternScanner;
        private readonly IPropertyManager _propertyManager;
        private readonly IDetourManager _detourManager;
        private readonly IDetourAttributeScanner _detourAttributeScanner;
        private readonly INamePoolService _namePoolService;

        public PatternScanner(ILogger logger, IEnginePattern enginePattern, IUObjectFuncs uObjectFuncs,
            IBulkTypePatternScanner bulkTypePatternScanner, IPropertyManager propertyManager,
            IDetourManager detourManager, IDetourAttributeScanner detourAttributeScanner, INamePoolService namePoolService) {
            _logger = logger;
            _loggerStatic = logger;
            _enginePattern = enginePattern;
            _uObjectFuncs = uObjectFuncs;
            _bulkTypePatternScanner = bulkTypePatternScanner;
            _propertyManager = propertyManager;
            _detourManager = detourManager;
            _detourAttributeScanner = detourAttributeScanner;
            _namePoolService = namePoolService;
            _namePoolServiceStatic = namePoolService;
        }

        public void ScanMemoryForUnrealReflectionPointers() {
            _logger.Debug("Starting Pattern Scan");
            var sw = Stopwatch.StartNew();

            _bulkTypePatternScanner
                .RegisterProperty(_enginePattern.GetType().GetProperty(nameof(_enginePattern.PGUObjectArray)), _enginePattern)
                .RegisterProperty(_enginePattern.GetType().GetProperty(nameof(_enginePattern.PNamePoolData)), _enginePattern)
                .RegisterProperty(_uObjectFuncs.GetType().GetProperty(nameof(_uObjectFuncs.GetExternalPackage)), _uObjectFuncs)
                .RegisterProperty(_uObjectFuncs.GetType().GetProperty(nameof(_uObjectFuncs.GetParentPackage)), _uObjectFuncs)
                //.RegisterProperty(_uObjectFuncs.GetType().GetProperty(nameof(_uObjectFuncs.GetFullName)), _uObjectFuncs);

                .RegisterDetour(GetType().GetMethod(nameof(UObjectBeginDestroy)))
                .RegisterDetour(GetType().GetMethod(nameof(UObjectFinishDestroy)))
                //.RegisterDetour(GetType().GetMethod(nameof(UObjectCtorDetour)))
                .RegisterDetour(GetType().GetMethod(nameof(ProcessEvent)))
                //.RegisterDetour(GetType().GetMethod(nameof(UObjectCtor)))
                .RegisterDetour(GetType().GetMethod(nameof(UObjectPostInitProperties)))


                .ScanAll()
                .UpdatePropertyValues(_propertyManager)
                .PrepareDetours(_detourAttributeScanner, _detourManager)
                .InstallDetours(_detourManager);

            sw.Stop();
            _logger.Debug($"Scanning took {sw.ElapsedMilliseconds} ms.");
        }

        private static HashSet<nint> LoadedObjects = new();
        private static HashSet<string> FuncNames = new();

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectPostInitProperties_Original;

        [Detour("48 83 ? ? 48 ? ? 48 C7 44 24 20 00 00 ? ? 48 8B ? ? 45 ? ? 41 ? ?", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectPostInitProperties(UObject* instance) {
            UObjectPostInitProperties_Original(instance);

            lock (LoadedObjects) {
                LoadedObjects.Add((nint)instance);
                Console.Title = $"Objects: {LoadedObjects.Count}";
            }
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectBeginDestroy_Original;

        [Detour("40 ? 48 83 ? ? 8B ? ? 48 ? ? C1 ? ? ? ? 75 ? 48 8B ? ? 48 8D 54", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectBeginDestroy(UObject* instance) {
            UObjectBeginDestroy_Original(instance);
        }


        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectFinishDestroy_Original;

        [Detour("40 ? 48 83 ? ? F6 41 ? ? 48 ? ? 75 ? 48 8B ? ? 48 8D 54 ? ? 48 8D 4C", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectFinishDestroy(UObject* instance) {
            lock (LoadedObjects) {
                var removed = LoadedObjects.Remove((nint)instance);
                Console.Title = $"Objects: {LoadedObjects.Count}";
            }

            UObjectFinishDestroy_Original(instance);
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, UFunction*, void*, void> ProcessEvent_Original;

        [Detour("40 ? ? ? 41 ? 41 ? 41 ? 41 ? 48 81 EC 10 01 ? ? 48 8D 6C ? ? 48 89 9D 38 01", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void ProcessEvent(UObject* instance, UFunction* uFunction, void* voidPtr) {
            if (uFunction is not null && instance is not null) {
                var className = _namePoolServiceStatic.GetNameString(instance->baseObjectBaseUtility.baseUObjectBase.classPrivate->ObjectName);
                var functionName = _namePoolServiceStatic.GetNameString(uFunction->baseUstruct.ObjectName);
                var implulseName = $"{className}::{functionName}";

                if (FuncNames.Add(functionName)) {
                    _loggerStatic.Info($"VM: {implulseName}");
                }

                if (!implulseName.Equals("PalHate::DamageEvent")) {
                    ProcessEvent_Original(instance, uFunction, voidPtr);
                }

                return;
            }

            ProcessEvent_Original(instance, uFunction, voidPtr);
        }
    }
}
