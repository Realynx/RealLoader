namespace RealLoaderInstaller.Services.Interfaces {
    public interface IGithubArtifactDownloader {
        Task<byte[]> DownloadGithubReleaseAsync(string githubFileName);
        Task<string> GetLatestVersion();
        Task<bool> IsOutOfDate(string assemblyImage);
        Task UnzipFrameworkPackage(string extractPath);
    }
}