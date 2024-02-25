namespace PalworldManagedModFramework.Sdk.Services.Detour.Models {
    public enum DetourType {
        Stack,
        Jmp_IP,
        VTable
    }

    public enum EngineFunction {
        ProccessEvent,
        UObject_PostInitProperties,
        UObject_BeginDestroy,
        UObject_FinishDestroy
    }
}
