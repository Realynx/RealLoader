using System.Diagnostics;
using System.IO.Compression;

using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services.Interfaces;

using Spectre.Console;

namespace RealLoaderInstaller.Services {
    public class GithubArtifactDownloader : IGithubArtifactDownloader {
        internal const string FRAMEWORK_ZIP = "RealLoaderFramework.zip";
        internal const string WINHTTP_PROXY = "winhttp.dll";
        internal const string CLR_HOST_WINDOWS = "CLRHost.dll";
        internal const string CLR_HOST_LINUX = "libCLRHost.so";

        private readonly HttpClient _httpClient;
        private readonly InstallerOptions _installerOptions;

        public GithubArtifactDownloader(HttpClient httpClient, InstallerOptions installerOptions) {
            _httpClient = httpClient;
            _installerOptions = installerOptions;

            _httpClient.BaseAddress = new Uri($"{_installerOptions.RemoteSource}/releases/latest/download/");
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

        public async Task<bool> IsOutOfDate(string assemblyImage) {
            var assemblyVersionInfo = FileVersionInfo.GetVersionInfo(assemblyImage);
            if (!Version.TryParse(assemblyVersionInfo.ProductVersion, out var assemblyVersion)) {
                AnsiConsole.WriteLine($"Failed to check identify current installed version.");
                return true;
            }

            var githubVersionString = await GetLatestVersion();
            var githubVersion = Version.Parse(githubVersionString.TrimStart('v', 'V'));
            return assemblyVersion < githubVersion;
        }

        public async Task<string> GetLatestVersion() {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            var response = await _httpClient.SendAsync(httpRequestMessage);

            var redirectLocation = response.RequestMessage.RequestUri.ToString();
            var versionString = redirectLocation.Substring(redirectLocation.LastIndexOf("/") + 1);

            return versionString;
        }
    }
}
