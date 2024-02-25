using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook {
    public class UnrealEventRegistrationService : IUnrealEventRegistrationService {
        private readonly ILogger _logger;
        private readonly IUnrealHookManager _unrealHookManager;
        private readonly IServiceProvider _serviceProvider;

        public UnrealEventRegistrationService(ILogger logger, IUnrealHookManager unrealHookManager, IServiceProvider serviceProvider) {
            _logger = logger;
            _unrealHookManager = unrealHookManager;
            _serviceProvider = serviceProvider;
        }

        public IUnrealEventRegistrationService FindAndRegisterEvents<TType>() {
            var parentType = typeof(TType);

            var parentInstance = _serviceProvider.GetService(parentType);
            return parentInstance is null ? this : FindAndRegisterEvents<TType>(parentInstance);
        }

        public IUnrealEventRegistrationService FindAndRegisterEvents<TType>(object parentInstance) {
            var parentType = parentInstance.GetType();

            var eventMethods = FindValidEventMethods<EngineEventAttribute>(parentType);
            foreach (var eventMethod in eventMethods) {
                _unrealHookManager.RegisterUnrealEvent(eventMethod, parentInstance);
            }

            return this;
        }

        public IUnrealEventRegistrationService FindAndRegisterEventHooks<TType>() {
            var parentType = typeof(TType);
            var parentInstance = _serviceProvider.GetService(parentType);
            return parentInstance is null ? this : FindAndRegisterEventHooks<TType>(parentInstance);
        }

        public IUnrealEventRegistrationService FindAndRegisterEventHooks<TType>(object parentInstance) {
            var parentType = parentInstance.GetType();

            var eventMethods = FindValidEventMethods<HookEngineEventAttribute>(parentType);
            foreach (var eventMethod in eventMethods) {
                _unrealHookManager.RegisterUnrealHook(eventMethod, parentInstance);
            }

            return this;
        }

        private IEnumerable<MethodInfo> FindValidEventMethods<TAttribute>(Type parentType) where TAttribute : Attribute {
            var methods = parentType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods) {
                var eventAttribute = method.GetCustomAttribute<TAttribute>();
                if (eventAttribute is null) {
                    continue;
                }

                yield return method;
            }
        }
    }
}
