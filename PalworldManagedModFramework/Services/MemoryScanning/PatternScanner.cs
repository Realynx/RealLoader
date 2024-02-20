using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory;
using PalworldManagedModFramework.Sdk.Services.Memory.Extensions;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.Flags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class PatternScanner {
        private static ILogger LoggerStatic;

        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly IUObjectFuncs _uObjectFuncs;
        private readonly IBulkTypePatternScanner _bulkTypePatternScanner;
        private readonly IPropertyManager _propertyManager;
        private readonly IDetourManager _detourManager;
        private readonly IDetourAttributeScanner _detourAttributeScanner;

        public PatternScanner(ILogger logger, IEnginePattern enginePattern, IUObjectFuncs uObjectFuncs,
            IBulkTypePatternScanner bulkTypePatternScanner, IPropertyManager propertyManager, IDetourManager detourManager, IDetourAttributeScanner detourAttributeScanner) {
            _logger = logger;
            LoggerStatic = logger;
            _enginePattern = enginePattern;
            _uObjectFuncs = uObjectFuncs;
            _bulkTypePatternScanner = bulkTypePatternScanner;
            _propertyManager = propertyManager;
            _detourManager = detourManager;
            _detourAttributeScanner = detourAttributeScanner;
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

                .RegisterDetour(GetType().GetMethod(nameof(UStructCtorDetour)))
                .RegisterDetour(GetType().GetMethod(nameof(UObjectCtorDetour)))

                .ScanAll()
                .UpdatePropertyValues(_propertyManager)
                .PrepareDetours(_detourAttributeScanner, _detourManager)
                .InstallDetours(_detourManager);

            sw.Stop();
            _logger.Debug($"Scanning took {sw.ElapsedMilliseconds} ms.");
        }

        public static unsafe delegate* unmanaged<UStruct*, int, int, int, EObjectFlags, UStruct*> UStructCtorDetour_Original;

        [Detour("48 89 5C ? ? 48 89 74 ? ? ? 48 83 ? ? 41 ? ? ? ? 44 8B 44 ? ? 41 ? ? 48 ? ? E8 1C 39 09", DetourType.Stack), UnmanagedCallersOnly(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe UStruct* UStructCtorDetour(UStruct* instance, int ctorFlags, int param2, int param3, EObjectFlags param4) {
            Console.WriteLine("CALLED WORKS");

            var constructedStruct = UStructCtorDetour_Original(instance, ctorFlags, param2, param3, param4);
            LoggerStatic.Info($"Constructed 0x{(nint)instance:X}");

            return constructedStruct;
        }

        public static unsafe delegate* unmanaged<UObject*, UObject*> UObjectCtorDetour_Original;

        [Detour("48 89 5C ? ? ? 48 83 ? ? 48 8D 05 CF 9A ? ? 48 ? ? 48 ? ? E8 14 BC FA ? 48 8D 44 ? ? C6 44 24 ? ?", DetourType.Stack), UnmanagedCallersOnly(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe UObject* UObjectCtorDetour(UObject* instance) {
            Console.WriteLine("CALLED WORKS");

            var constructedObject = UObjectCtorDetour_Original(instance);
            LoggerStatic.Info($"Constructed 0x{(nint)instance:X}");

            return constructedObject;
        }
    }
}
