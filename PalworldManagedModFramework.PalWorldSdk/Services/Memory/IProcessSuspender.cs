namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    public interface IProcessSuspender {
        void PauseSelf();
        void ResumeSelf();
    }
}