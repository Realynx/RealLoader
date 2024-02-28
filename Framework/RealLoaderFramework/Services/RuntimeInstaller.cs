using System.Diagnostics;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Detour.Extensions;
using RealLoaderFramework.Sdk.Services.Detour.Interfaces;
using RealLoaderFramework.Sdk.Services.EngineServices;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook;
using RealLoaderFramework.Sdk.Services.Memory.Extensions;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Services.Interfaces;

namespace RealLoaderFramework.Services {
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
