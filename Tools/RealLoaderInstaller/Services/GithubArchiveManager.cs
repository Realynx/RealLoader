using System.IO.Compression;

using RealLoaderInstaller.Services.Interfaces;

namespace RealLoaderInstaller.Services {
    public class GithubArchiveManager : IGithubArchiveManager {
        internal const string FRAMEWORK_ZIP = "RealLoaderFramework.zip";
        internal const string WINDOWS_ARTIFACTS_ZIP = "WindowsArtifacts.zip";
        internal const string LINUX_ARTIFACTS_ZIP = "LinuxArtifacts.zip";
        internal const string SDK_BUILDER_MOD = "DotNetSdkBuilderMod.dll";

        private readonly IGithubArtifactDownloader _githubArtifactDownloader;
        public struct GithubArchive {
            public DirectoryInfo tempDirectory;

            public byte[] this[string index] {
                get {
                    return File.ReadAllBytes(Path.Combine(tempDirectory.FullName, index));
                }
            }
        }

        public GithubArchiveManager(IGithubArtifactDownloader githubArtifactDownloader) {
            _githubArtifactDownloader = githubArtifactDownloader;
        }

        public async Task<GithubArchive> CheckoutGithubArchive(string archiveName) {
            var zipFileBytes = await _githubArtifactDownloader.DownloadGithubReleaseAsync(archiveName);
            var unpackdirectory = Directory.CreateTempSubdirectory("RealLoaderInstaller_");

            var zipFilePath = Path.Combine(unpackdirectory.FullName, archiveName);
            File.WriteAllBytes(zipFilePath, zipFileBytes);

            using var archive = ZipFile.OpenRead(zipFilePath);
            archive.ExtractToDirectory(unpackdirectory.FullName, true);

            return new GithubArchive {
                tempDirectory = unpackdirectory
            };
        }
    }
}
