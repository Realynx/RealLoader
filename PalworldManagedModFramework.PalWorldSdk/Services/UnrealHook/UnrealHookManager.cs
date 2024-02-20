using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Sdk.Services.UnrealHook {
    public class UnrealHookManager {
        public UnrealHookManager(ILogger logger, INamePoolService namePoolService) {
            _logger = logger;
            _namePoolService = namePoolService;
        }

        private static ILogger _logger;
        private static INamePoolService _namePoolService;

        private static HashSet<string> FuncNames = new();


        public void RegisterUnrealHook(HookEngineEventAttribute hookEngineEventAttribute) {

        }

        public void RegisterUnrealEvent(EngineEventAttribute engineEventAttribute) {

        }


        public static unsafe delegate* unmanaged[Thiscall]<UObject*, UFunction*, void*, void> ProcessEvent_Original;
        [Detour("40 ? ? ? 41 ? 41 ? 41 ? 41 ? 48 81 EC 10 01 ? ? 48 8D 6C ? ? 48 89 9D 38 01", DetourType.Stack)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        public static unsafe void ProcessEvent(UObject* instance, UFunction* uFunction, void* voidPtr) {
            if (uFunction is not null && instance is not null) {

                //var executingEvent = new UnrealEventRecord(ref Unsafe.AsRef<UObject>(instance), ref Unsafe.AsRef<UFunction>(uFunction), ref Unsafe.AsRef<object>(voidPtr),
                //    (ref UObject instance, ref UFunction uFunction, ref object voidPtr)
                //        => ProcessEvent_Original((UObject*)Unsafe.AsPointer(ref instance), (UFunction*)Unsafe.AsPointer(ref uFunction), Unsafe.AsPointer(ref voidPtr)));


                //var className = _namePoolService.GetNameString(instance->baseObjectBaseUtility.baseUObjectBase.classPrivate->ObjectName);
                //var functionName = _namePoolService.GetNameString(uFunction->baseUstruct.ObjectName);
                //var eventName = $"{className}::{functionName}";

                //if (FuncNames.Add(functionName)) {
                //    _logger.Info($"VM Event: {eventName}");
                //}

                //if (!eventName.Equals("PalHate::DamageEvent")) {
                //    ProcessEvent_Original(instance, uFunction, voidPtr);
                //}

                //return;
            }

            ProcessEvent_Original(instance, uFunction, voidPtr);
        }
    }
}
