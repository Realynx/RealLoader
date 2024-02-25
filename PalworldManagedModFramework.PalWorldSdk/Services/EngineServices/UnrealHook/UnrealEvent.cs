using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook {
    public unsafe class UnrealEvent {
        public delegate void ExecuteOriginalCallback(UObject* target, UFunction* function, void* parameter);

        public unsafe UnrealEvent(string eventName, UObject* instance, UFunction* uFunction, void* parameters) {
            EventName = eventName;
            Instance = instance;
            UFunction = uFunction;
            Params = parameters;
        }


        public string EventName { get; set; }
        public UObject* Instance;
        public UFunction* UFunction;
        public void* Params;
        public bool ContinueExecute { get; set; } = true;
    }
}
