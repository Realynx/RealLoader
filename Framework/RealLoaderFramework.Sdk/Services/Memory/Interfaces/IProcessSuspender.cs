namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface IProcessSuspender {
        void PauseSelf();
        void ResumeSelf();
    }
}