using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.FunctionServices;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.EngineServices;
using PalworldManagedModFramework.Sdk.Services.Memory.Extensions;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.UnrealHook.Interfaces;
using PalworldManagedModFramework.Services.Interfaces;

namespace PalworldManagedModFramework.Services {
    internal class RuntimeInstaller : IRuntimeInstaller {

        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly IUObjectFuncs _uObjectFuncs;
        private readonly IBulkTypePatternScanner _bulkTypePatternScanner;
        private readonly IPropertyManager _propertyManager;
        private readonly IDetourManager _detourManager;
        private readonly IDetourAttributeScanner _detourAttributeScanner;
        private readonly IGlobalObjectsTracker _globalObjectsTracker;
        private readonly IUnrealHookManager _unrealHookManager;

        public RuntimeInstaller(ILogger logger, IEnginePattern enginePattern, IUObjectFuncs uObjectFuncs, IBulkTypePatternScanner bulkTypePatternScanner,
            IPropertyManager propertyManager, IDetourManager detourManager, IDetourAttributeScanner detourAttributeScanner, IGlobalObjectsTracker globalObjectsTracker, IUnrealHookManager unrealHookManager) {

            _logger = logger;
            _enginePattern = enginePattern;
            _uObjectFuncs = uObjectFuncs;
            _bulkTypePatternScanner = bulkTypePatternScanner;
            _propertyManager = propertyManager;
            _detourManager = detourManager;
            _detourAttributeScanner = detourAttributeScanner;
            _globalObjectsTracker = globalObjectsTracker;
            _unrealHookManager = unrealHookManager;
        }

        /// <summary>
        /// TODO: Refactor this, it has too many dependencies & responsibilities
        /// </summary>
        public void ScanAndInstallRuntime() {
            _logger.Debug("Starting Pattern Scan");
            var sw = Stopwatch.StartNew();

            _bulkTypePatternScanner
                .RegisterProperty(_enginePattern.GetType().GetProperty(nameof(_enginePattern.PGUObjectArray)), _enginePattern)
                .RegisterProperty(_enginePattern.GetType().GetProperty(nameof(_enginePattern.PNamePoolData)), _enginePattern)
                .RegisterProperty(_uObjectFuncs.GetType().GetProperty(nameof(_uObjectFuncs.GetExternalPackage)), _uObjectFuncs)
                .RegisterProperty(_uObjectFuncs.GetType().GetProperty(nameof(_uObjectFuncs.GetParentPackage)), _uObjectFuncs)
                //.RegisterProperty(_uObjectFuncs.GetType().GetProperty(nameof(_uObjectFuncs.GetFullName)), _uObjectFuncs);

                .RegisterDetour(_globalObjectsTracker.GetType().GetMethod(nameof(GlobalObjectsTracker.UObjectBeginDestroy)))
                .RegisterDetour(_globalObjectsTracker.GetType().GetMethod(nameof(GlobalObjectsTracker.UObjectFinishDestroy)))
                .RegisterDetour(_unrealHookManager.GetType().GetMethod(nameof(UnrealHookManager.ProcessEvent)))
                .RegisterDetour(_globalObjectsTracker.GetType().GetMethod(nameof(GlobalObjectsTracker.UObjectPostInitProperties)))

                .ScanAll()
                .UpdatePropertyValues(_propertyManager)
                .PrepareDetours(_detourAttributeScanner, _detourManager)
                .InstallDetours(_detourManager);

            sw.Stop();
            _logger.Debug($"Scanning took {sw.ElapsedMilliseconds} ms.");
        }
    }
}
