using System.IO.Compression;

using PalworldModInstaller.Services.Interfaces;

using Spectre.Console;

namespace PalworldModInstaller.Services {
    public class GithubArtifactDownloader : IGithubArtifactDownloader {
        internal const string FRAMEWORK_ZIP = "ManagedModFramework.zip";
        internal const string WINHTTP_PROXY = "winhttp.dll";
        internal const string CLR_HOST_WINDOWS = "CLRHost.dll";
        internal const string CLR_HOST_LINUX = "libCLRHost.so";

        private readonly HttpClient _httpClient;

        public GithubArtifactDownloader(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        public async Task<byte[]> DownloadGithubReleaseAsync(string githubFileName) {
            var httpFileResponse = await _httpClient.GetAsync(githubFileName);
            if (!httpFileResponse.IsSuccessStatusCode) {
                AnsiConsole.WriteLine($"Failed to download '{githubFileName}'...");
            }

            var httpFileBytes = await httpFileResponse.Content.ReadAsByteArrayAsync();
            return httpFileBytes;
        }

        public async Task UnzipFrameworkPackage(string extractPath) {
            var zipFileBytes = await DownloadGithubReleaseAsync(FRAMEWORK_ZIP);
            await File.WriteAllBytesAsync(FRAMEWORK_ZIP, zipFileBytes);

            using var archive = ZipFile.OpenRead(FRAMEWORK_ZIP);
            foreach (var entry in archive.Entries) {
                var destinationPath = Path.Combine(extractPath, entry.FullName);
                entry.ExtractToFile(destinationPath, overwrite: true);
            }
        }

        public async Task<string> GetLatestVersion() {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            var response = _httpClient.Send(httpRequestMessage);

            var redirectLocation = response.RequestMessage.RequestUri.ToString();
            var versionString = redirectLocation.Substring(redirectLocation.LastIndexOf("/") + 1);

            return versionString;
        }
    }
}
