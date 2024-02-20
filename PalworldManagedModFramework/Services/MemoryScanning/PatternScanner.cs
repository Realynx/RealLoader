using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory;
using PalworldManagedModFramework.Sdk.Services.Memory.Extensions;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.Flags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class PatternScanner {
        private static ILogger LoggerStatic;
        private static INamePoolService NamePoolService;

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
            LoggerStatic = logger;
            _enginePattern = enginePattern;
            _uObjectFuncs = uObjectFuncs;
            _bulkTypePatternScanner = bulkTypePatternScanner;
            _propertyManager = propertyManager;
            _detourManager = detourManager;
            _detourAttributeScanner = detourAttributeScanner;
            _namePoolService = namePoolService;
            NamePoolService = namePoolService;
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
                //.RegisterDetour(GetType().GetMethod(nameof(UObjectCtorDetour)))
                .RegisterDetour(GetType().GetMethod(nameof(ProcessEvent)))
                //.RegisterDetour(GetType().GetMethod(nameof(UObjectCtor)))
                .RegisterDetour(GetType().GetMethod(nameof(UObjectPostLoad)))


                .ScanAll()
                .UpdatePropertyValues(_propertyManager)
                .PrepareDetours(_detourAttributeScanner, _detourManager)
                .InstallDetours(_detourManager);

            sw.Stop();
            _logger.Debug($"Scanning took {sw.ElapsedMilliseconds} ms.");
        }

        private static HashSet<nint> LoadedObjects = new();

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectPostLoad_Original;

        [Detour("48 83 ? ? 48 ? ? 48 C7 44 24 20 00 00 ? ? 48 8B ? ? 45 ? ? 41 ? ?", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectPostLoad(UObject* instance) {
            UObjectPostLoad_Original(instance);

            lock (LoadedObjects) {
                LoadedObjects.Add((nint)instance);
                Console.Title = $"Objects: {LoadedObjects.Count}";
            }
        }


        //public static unsafe delegate* unmanaged[Thiscall]<UObject*, object*, UObject*> UObjectCtor_Original;

        //[Detour("48 89 5C ? ? ? 48 83 ? ? 48 8D 05 2F 9B ? ? 48 ? ? 48 ? ? 48 ? ? E8 71 BC FA ?", DetourType.Stack)]
        //[UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        //public static unsafe UObject* UObjectCtor(UObject* instance, object* initalizer) {
        //    var constructedObject = UObjectCtor_Original(instance, initalizer);

        //    LoggerStatic.Info($"Constructed 0x{(nint)instance:X}");

        //    return constructedObject;
        //}

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, void> UObjectBeginDestroy_Original;

        [Detour("40 ? 48 83 ? ? 8B ? ? 48 ? ? C1 ? ? ? ? 75 ? 48 8B ? ? 48 8D 54", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void UObjectBeginDestroy(UObject* instance) {
            UObjectBeginDestroy_Original(instance);

            lock (LoadedObjects) {
                LoadedObjects.Remove((nint)instance);
                Console.Title = $"Objects: {LoadedObjects.Count}";
            }
        }

        //public static unsafe delegate* unmanaged[Thiscall]<UObject*, UObject*> UObjectCtorDetour_Original;

        //[Detour("48 89 5C ? ? ? 48 83 ? ? 48 8D 05 CF 9A ? ? 48 ? ? 48 ? ? E8 14 BC FA ? 48 8D 44", DetourType.Stack)]
        //[UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        //public static unsafe UObject* UObjectCtorDetour(UObject* instance) {
        //    var constructedObject = UObjectCtorDetour_Original(instance);
        //    LoggerStatic.Info($"Constructed 0x{(nint)instance:X}");

        //    return constructedObject;
        //}

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, UFunction*, void*, void> ProcessEvent_Original;

        [Detour("40 ? ? ? 41 ? 41 ? 41 ? 41 ? 48 81 EC 10 01 ? ? 48 8D 6C ? ? 48 89 9D 38 01", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void ProcessEvent(UObject* instance, UFunction* uFunction, void* voidPtr) {
            ProcessEvent_Original(instance, uFunction, voidPtr);

            //var functionName = "Null Func";
            if (uFunction is not null) {
                var functionName = NamePoolService.GetNameString(uFunction->baseUstruct.ObjectName);
                if (FuncNames.Add(functionName)) {
                    LoggerStatic.Info($"VM: {functionName}");
                }
            }

        }

        private static HashSet<string> FuncNames = new();
    }
}
