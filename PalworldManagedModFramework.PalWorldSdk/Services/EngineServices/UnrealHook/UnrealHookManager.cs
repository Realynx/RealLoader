using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

using static PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook.UnrealEvent;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook {
    public class UnrealHookManager : IUnrealHookManager {
        protected static UnrealHookManager? SingleInstance = null!;

        private readonly ILogger _logger;
        private readonly INamePoolService _namePoolService;
        private readonly IStackWalker _stackWalker;

        private readonly Dictionary<string, MethodInfo> _hookEvents = new();
        private readonly Dictionary<Regex, MethodInfo> _eventImpulses = new();
        private readonly Dictionary<MethodInfo, object> _methodInstances = new();

        public UnrealHookManager(ILogger logger, INamePoolService namePoolService, IStackWalker stackWalker) {
            _logger = logger;
            _namePoolService = namePoolService;
            _stackWalker = stackWalker;
            _logger.Debug($"Setup single Instance {nameof(SingleInstance)}");
            SingleInstance = this;
        }

        public IUnrealHookManager RegisterUnrealHook(MethodInfo hookEngineEventMethod, object instance) {
            var hookEngineEventAttribute = hookEngineEventMethod.GetCustomAttribute<HookEngineEventAttribute>()
                ?? throw new Exception($"Engine Hook Was Invalid! '{hookEngineEventMethod.Name}'");

            _methodInstances.Add(hookEngineEventMethod, instance);
            _hookEvents.Add(hookEngineEventAttribute.FullyQualifiedName, hookEngineEventMethod);
            return this;
        }

        public IUnrealHookManager RegisterUnrealEvent(MethodInfo engineEventMethod, object instance) {
            var hookEngineEventAttribute = engineEventMethod.GetCustomAttribute<EngineEventAttribute>()
                ?? throw new Exception($"Engine Event Was Invalid! '{engineEventMethod.Name}'");

            _methodInstances.Add(engineEventMethod, instance);
            _eventImpulses.Add(hookEngineEventAttribute.EventMask, engineEventMethod);
            return this;
        }

        private unsafe void OnUnrealEvent(UnrealEvent unrealEvent, ExecuteOriginalCallback executeOriginalCallback) {
            Task.Factory.StartNew(() => {
                var eventMasks = _eventImpulses.Keys.ToArray();
                var eventsToImpulse = eventMasks.Where(i => i.IsMatch(unrealEvent.EventName));

                foreach (var eventToImpulse in eventsToImpulse) {
                    var methodInfo = _eventImpulses[eventToImpulse];
                    var methodInstance = _methodInstances[methodInfo];
                    methodInfo.Invoke(methodInstance, [unrealEvent]);
                }
            });

            if (_hookEvents.ContainsKey(unrealEvent.EventName)) {
                var methodInfo = _hookEvents[unrealEvent.EventName];
                var methodInstance = _methodInstances[methodInfo];

                methodInfo.Invoke(methodInstance, [unrealEvent, executeOriginalCallback]);
            }

            if (unrealEvent.ContinueExecute) {
                executeOriginalCallback(unrealEvent.Instance, unrealEvent.UFunction, unrealEvent.Params);
            }
        }

        public static unsafe delegate* unmanaged[Thiscall]<UObject*, UFunction*, void*, void> ProcessEvent_Original;
        [EngineDetour(EngineFunction.ProccessEvent, DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void ProcessEvent(UObject* instance, UFunction* uFunction, void* parameters) {
            if (SingleInstance is null || uFunction is null || instance is null) {
                ProcessEvent_Original(instance, uFunction, parameters);
                return;
            }

            var className = SingleInstance._namePoolService
                .GetNameString(instance->baseObjectBaseUtility.baseUObjectBase.classPrivate->ObjectName);

            var functionName = SingleInstance._namePoolService.GetNameString(uFunction->baseUstruct.ObjectName);
            var eventName = $"{className}::{functionName}";

            var executingEvent = new UnrealEvent(eventName, instance, uFunction, parameters);

            SingleInstance.OnUnrealEvent(executingEvent, (pInstance, pUFunction, pParams)
                    => ProcessEvent_Original(pInstance, pUFunction, pParams));
        }
    }
}
