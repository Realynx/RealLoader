using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;
using RealLoaderFramework.Sdk.Services.Detour.Models;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;
using RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;

using static RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook.UnrealEvent;

namespace RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook {
    public class UnrealHookManager : IUnrealHookManager {
        private static UnrealHookManager? _singleInstance = null!;

        private readonly ILogger _logger;
        private readonly INamePoolService _namePoolService;
        private readonly IStackWalker _stackWalker;

        private readonly Dictionary<string, HashSet<MethodInfo>> _hookEvents = new();
        private readonly Dictionary<Regex, HashSet<MethodInfo>> _eventImpulses = new();
        private readonly Dictionary<MethodInfo, object> _methodInstances = new();

        public UnrealHookManager(ILogger logger, INamePoolService namePoolService, IStackWalker stackWalker) {
            _logger = logger;
            _namePoolService = namePoolService;
            _stackWalker = stackWalker;
            _logger.Debug($"Setup single Instance {nameof(_singleInstance)}");
            _singleInstance = this;
        }

        public IUnrealHookManager RegisterUnrealHook(MethodInfo hookEngineEventMethod, object instance) {
            var hookEngineEventAttribute = hookEngineEventMethod.GetCustomAttribute<HookEngineEventAttribute>()
                ?? throw new Exception($"Engine Hook Was Invalid! '{hookEngineEventMethod.Name}'");

            ref var hookMethods = ref CollectionsMarshal.GetValueRefOrAddDefault(_hookEvents, hookEngineEventAttribute.FullyQualifiedName, out var previouslyExisted);
            if (!previouslyExisted) {
                hookMethods = new HashSet<MethodInfo>();
            }

            _methodInstances.Add(hookEngineEventMethod, instance);
            if (!hookMethods!.Add(hookEngineEventMethod)) {
                _logger.Warning($"Attempted to register {hookEngineEventMethod.DeclaringType?.Name ?? "Private class"}.{hookEngineEventMethod.Name} for the same hook more than once.");
            }

            return this;
        }

        public IUnrealHookManager RegisterUnrealEvent(MethodInfo engineEventMethod, object instance) {
            var hookEngineEventAttribute = engineEventMethod.GetCustomAttribute<EngineEventAttribute>()
                ?? throw new Exception($"Engine Event Was Invalid! '{engineEventMethod.Name}'");

            ref var impulseMethods = ref CollectionsMarshal.GetValueRefOrAddDefault(_eventImpulses, hookEngineEventAttribute.EventMask, out var previouslyExisted);
            if (!previouslyExisted) {
                impulseMethods = new HashSet<MethodInfo>();
            }

            _methodInstances.Add(engineEventMethod, instance);
            if (!impulseMethods!.Add(engineEventMethod)) {
                _logger.Warning($"Attempted to register {engineEventMethod.DeclaringType?.Name ?? "Private class"}.{engineEventMethod.Name} for the same pattern more than once.");
            }

            return this;
        }

        // TODO: We might want to create delegates instead of invoking via MethodInfo to reduce overhead
        private unsafe void OnUnrealEvent(UnrealEvent unrealEvent, ExecuteOriginalCallback executeOriginalCallback) {
            Task.Factory.StartNew(() => {
                object[]? parameterizedUnrealEvent = null;

                foreach (var (eventMask, methodInfos) in _eventImpulses) {
                    if (!eventMask.IsMatch(unrealEvent.EventName)) {
                        continue;
                    }

                    foreach (var methodInfo in methodInfos) {
                        parameterizedUnrealEvent ??= [unrealEvent];
                        var methodInstance = _methodInstances[methodInfo];
                        methodInfo.Invoke(methodInstance, parameterizedUnrealEvent);
                    }
                }
            });

            if (_hookEvents.TryGetValue(unrealEvent.EventName, out var methodInfos1)) {
                object[]? parameters = null;

                foreach (var methodInfo in methodInfos1) {
                    parameters ??= [unrealEvent, executeOriginalCallback];
                    var methodInstance = _methodInstances[methodInfo];
                    methodInfo.Invoke(methodInstance, parameters);
                }
            }

            if (unrealEvent.ContinueExecute) {
                executeOriginalCallback(unrealEvent.Instance, unrealEvent.UFunction, unrealEvent.Params);
            }
        }

        /// <summary>
        /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Private/UObject/ScriptCore.cpp#L1963"/>
        /// </summary>
        public static unsafe delegate* unmanaged[Thiscall]<UObject*, UFunction*, void*, void> ProcessEvent_Original;
        [EngineDetour(EngineFunction.ProcessEvent, DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void ProcessEvent(UObject* instance, UFunction* uFunction, void* parameters) {
            if (_singleInstance is null || uFunction is null || instance is null) {
                ProcessEvent_Original(instance, uFunction, parameters);
                return;
            }

            var className = _singleInstance._namePoolService
                .GetNameString(instance->baseObjectBaseUtility.baseUObjectBase.classPrivate->ObjectName);

            var functionName = _singleInstance._namePoolService.GetNameString(uFunction->baseUstruct.ObjectName);
            var eventName = $"{className}::{functionName}";

            var executingEvent = new UnrealEvent(eventName, instance, uFunction, parameters);

            _singleInstance.OnUnrealEvent(executingEvent, (pInstance, pUFunction, pParams)
                => ProcessEvent_Original(pInstance, pUFunction, pParams));
        }
    }
}
