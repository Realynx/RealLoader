namespace UnrealCoreObjectApiSourceGen.Services.Interfaces {
    public interface IPrivateGithubReader {
        Task<string> DownloadHeaderFile(string engineVersion, string headerFile);
    }
}