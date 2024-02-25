namespace PalworldModInstaller.Services.Interfaces {
    public interface IModFileService {
        void CheckClrModsFolder(string modsFolder);
        void CheckFrameworkInstallFolder(string dotnetDependenciesFolder);
        Task InstallNewFiles(string dotnetDependenciesFolder, string clrHost);
        Task WriteGithubFile(string localLocation, string githubFilename);
    }
}