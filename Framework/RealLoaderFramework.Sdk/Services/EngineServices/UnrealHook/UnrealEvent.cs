using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace RealLoaderFramework.Sdk.Services.EngineServices.UnrealHook {
    public unsafe class UnrealEvent {
        public delegate void ExecuteOriginalCallback(UObject* target, UFunction* function, void* parameter);

        public UnrealEvent(string eventName, UObject* instance, UFunction* uFunction, void* parameters) {
            EventName = eventName;
            Instance = instance;
            UFunction = uFunction;
            Params = parameters;
        }

        public string EventName { get; }
        public UObject* Instance { get; }
        public UFunction* UFunction { get; }
        public void* Params { get; }
        public bool ContinueExecute { get; set; } = true;
    }
}
