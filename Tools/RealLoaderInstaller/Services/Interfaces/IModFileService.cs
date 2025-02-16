namespace RealLoaderInstaller.Services.Interfaces {
    public interface IModFileService {
        void CheckClrModsFolder(string modsFolder);
        void CheckFrameworkInstallFolder(string dotnetDependenciesFolder);
        Task WriteGithubFile(string localLocation, string githubFilename);
    }
}