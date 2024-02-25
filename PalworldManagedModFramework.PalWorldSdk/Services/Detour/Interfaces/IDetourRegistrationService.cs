namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public interface IDetourRegistrationService {
        IDetourRegistrationService FindAndRegisterDetours<TType>();
    }
}