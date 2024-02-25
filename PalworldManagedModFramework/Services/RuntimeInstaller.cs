using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Extensions;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.EngineServices;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook;
using PalworldManagedModFramework.Sdk.Services.Memory.Extensions;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Services.Interfaces;

namespace PalworldManagedModFramework.Services {
    internal class RuntimeInstaller : IRuntimeInstaller {

        private readonly ILogger _logger;
        private readonly IBulkPatternScanner _bulkTypePatternScanner;
        private readonly IPropertyManager _propertyManager;
        private readonly IDetourManager _detourManager;
        private readonly IDetourAttributeService _detourAttributeScanner;
        private readonly IDetourRegistrationService _detourRegistrationService;
        private readonly IPropertyRegistrationService _propertyRegistrationService;

        public RuntimeInstaller(ILogger logger, IBulkPatternScanner bulkTypePatternScanner, IPropertyManager propertyManager,
            IDetourManager detourManager, IDetourAttributeService detourAttributeScanner, IDetourRegistrationService detourRegistrationService,
            IPropertyRegistrationService propertyRegistrationService) {
            _logger = logger;
            _bulkTypePatternScanner = bulkTypePatternScanner;
            _propertyManager = propertyManager;
            _detourManager = detourManager;
            _detourAttributeScanner = detourAttributeScanner;
            _detourRegistrationService = detourRegistrationService;
            _propertyRegistrationService = propertyRegistrationService;
        }

        /// <summary>
        /// TODO: Refactor this, it has too many dependencies & responsibilities
        /// </summary>
        public void ScanAndInstallRuntime() {
            _detourRegistrationService
                .FindAndRegisterDetours<GlobalObjectsTracker>()
                .FindAndRegisterDetours<UnrealHookManager>();

            _propertyRegistrationService
                .FindAndRegisterProperties<IEnginePattern>()
                .FindAndRegisterProperties<IUObjectFuncs>();

            _logger.Debug("Starting Bulk Pattern Scan");
            var sw = Stopwatch.StartNew();
            _bulkTypePatternScanner
                .ScanAll();
            sw.Stop();
            _logger.Debug($"Scanning took {sw.ElapsedMilliseconds} ms.");

            _bulkTypePatternScanner
                .UpdatePropertyValues(_propertyManager)
                .PrepareDetours(_detourAttributeScanner, _detourManager)
                .InstallDetours(_detourManager);
        }
    }
}
