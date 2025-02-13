namespace RealLoaderFramework.Sdk.Services.Detour.Models {
    public enum DetourType {
        Stack,
        Jmp_IP,
        VTable
    }

    public enum EngineFunction {
        ProcessEvent,
        UObject_PostInitProperties,
        UObject_BeginDestroy,
        UObject_FinishDestroy
    }
}
