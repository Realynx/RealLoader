using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.Flags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class PatternScanner {
        private static ILogger LoggerStatic;

        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly IPatternResolver _patternResolver;
        private readonly IUObjectFuncs _uObjectFuncs;

        public PatternScanner(ILogger logger, IEnginePattern enginePattern, IPatternResolver patternResolver, IUObjectFuncs uObjectFuncs) {
            _logger = logger;
            LoggerStatic = logger;
            _enginePattern = enginePattern;
            _patternResolver = patternResolver;
            _uObjectFuncs = uObjectFuncs;
        }

        public void ScanMemoryForUnrealReflectionPointers() {
            _logger.Debug("Starting Pattern Scan");

            using var bulkScanner = new BulkScanner(_patternResolver);
            bulkScanner
                .ResolveMachineCodeProperty(_enginePattern, nameof(_enginePattern.PGUObjectArray))
                .ResolveMachineCodeProperty(_enginePattern, nameof(_enginePattern.PNamePoolData))

                .ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetExternalPackage))
                .ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetParentPackage))
            //.ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetFullName));

            bulkScanner.ScanAllProperties();

            _patternResolver.ResolvePattern

        }

        public static unsafe delegate* unmanaged<UStruct*, int, int, int, EObjectFlags, void> UStructCtorHook_Original;
        [Hook("48 89 8E 80 00 ? ? 48 89 8E 88 00 ? ? 48 89 8E 90 00 ? ? 48 89 8E 98 00 ? ? 48 89 8E A0 00 ? ? 48 89 8E A8 00 ? ? 48 8B 74 ? ? 48 83 ? ? ? C3"), UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        public static unsafe void UStructCtorHook(UStruct* instance, int ctorFlags, int param2, int param3, EObjectFlags param4)
        {
            UStructCtorHook_Original(instance, ctorFlags, param2, param3, param4);
            LoggerStatic.Info($"Constructed 0x{(nint)instance:X}");
        }
    }
}
