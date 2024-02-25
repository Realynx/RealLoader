namespace PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook.Interfaces {
    public interface IUnrealEventRegistrationService {
        IUnrealEventRegistrationService FindAndRegisterEventHooks<TType>();
        IUnrealEventRegistrationService FindAndRegisterEventHooks<TType>(object parentInstance);
        IUnrealEventRegistrationService FindAndRegisterEvents<TType>();
        IUnrealEventRegistrationService FindAndRegisterEvents<TType>(object parentInstance);
    }
}