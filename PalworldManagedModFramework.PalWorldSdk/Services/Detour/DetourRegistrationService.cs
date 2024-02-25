using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Extensions;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public class DetourRegistrationService : IDetourRegistrationService {
        private readonly ILogger _logger;
        private readonly IDetourAttributeService _detourAttributeService;
        private readonly IEnginePattern _enginePattern;
        private readonly IBulkPatternScanner _bulkPatternScanner;
        private readonly IServiceProvider _serviceProvider;

        public DetourRegistrationService(ILogger logger, IDetourAttributeService detourAttributeService,
            IEnginePattern enginePattern, IBulkPatternScanner bulkPatternScanner, IServiceProvider serviceProvider) {
            _logger = logger;
            _detourAttributeService = detourAttributeService;
            _enginePattern = enginePattern;
            _bulkPatternScanner = bulkPatternScanner;
            _serviceProvider = serviceProvider;
        }

        public IDetourRegistrationService FindAndRegisterDetours<TType>() {
            var parentType = typeof(TType);

            var methods = parentType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            var validDetourInfos = FindDetourInfos(methods);
            foreach (var detourInfo in validDetourInfos) {
                var pattern = detourInfo.DetourAttribute switch {
                    PatternDetourAttribute patternDetourAttribute => patternDetourAttribute.Pattern,
                    EngineDetourAttribute engineDetourAttribute => GetEnginePattern(engineDetourAttribute),
                    _ => null
                };

                if (pattern is null) {
                    _logger.Error($"Invalid detour pattern, {detourInfo.DetourMethod.Name}");
                    return this;
                }

                _bulkPatternScanner.RegisterPatternDetour(detourInfo.DetourMethod, pattern);
            }

            return this;
        }

        private string? GetEnginePattern(EngineDetourAttribute engineDetourAttribute) => engineDetourAttribute.EngineFunction switch {
            EngineFunction.ProccessEvent => _enginePattern.ProcessEventPattern,
            EngineFunction.UObject_PostInitProperties => _enginePattern.UObject_PostInitPropertiesPattern,
            EngineFunction.UObject_BeginDestroy => _enginePattern.UObject_BeginDestroyPattern,
            EngineFunction.UObject_FinishDestroy => _enginePattern.UObject_FinishDestroyPattern,
            _ => null
        };

        private ICollection<ManagedDetourInfo> FindDetourInfos(MethodInfo[] methods) {
            var detoursForRegister = new List<ManagedDetourInfo>();

            foreach (var method in methods) {
                var detourInfo = _detourAttributeService.GetManagedDetourInfo(method);
                if (detourInfo is null) {
                    continue;
                }

                detoursForRegister.Add(detourInfo);
            }

            return detoursForRegister;
        }
    }
}
