namespace PalworldModInstaller.Services.Interfaces {
    public interface IGithubArtifactDownloader {
        Task<byte[]> DownloadGithubReleaseAsync(string githubFileName);
        Task<string> GetLatestVersion();
        Task UnzipFrameworkPackage(string extractPath);
    }
}