
namespace RealLoaderInstaller.Services {
    public interface IGithubArchiveManager {
        Task<GithubArchiveManager.GithubArchive> CheckoutGithubArchive(string archiveName);
    }
}