namespace RealLoaderFramework.Sdk.Services.EngineServices.Interfaces {
    public interface IEnginePattern {
        nint PNamePoolData { get; set; }
        nint PGUObjectArray { get; set; }
        string ProcessEventPattern { get; }
        string UObject_PostInitPropertiesPattern { get; }
        string UObject_BeginDestroyPattern { get; }
        string UObject_FinishDestroyPattern { get; }
    }
}
