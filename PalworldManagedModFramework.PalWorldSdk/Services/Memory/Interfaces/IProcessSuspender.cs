namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces {
    public interface IProcessSuspender {
        void PauseSelf();
        void ResumeSelf();
    }
}