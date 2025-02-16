using System.Diagnostics;
using System.IO.Compression;

using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services.Interfaces;

using Spectre.Console;

namespace RealLoaderInstaller.Services {
    public class GithubArtifactDownloader : IGithubArtifactDownloader {
        private readonly HttpClient _httpClient;
        private readonly InstallerOptions _installerOptions;

        public GithubArtifactDownloader(HttpClient httpClient, InstallerOptions installerOptions) {
            _httpClient = httpClient;
            _installerOptions = installerOptions;

            _httpClient.BaseAddress = new Uri($"{_installerOptions.RemoteSource}/releases/download/nightly-build/");
        }

        public async Task<byte[]> DownloadGithubReleaseAsync(string githubFileName) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, githubFileName);
            var httpFileResponse = await _httpClient.SendAsync(httpRequest);

            if (!httpFileResponse.IsSuccessStatusCode) {
                AnsiConsole.WriteLine($"Failed to download '{githubFileName}'...");
            }

            var httpFileBytes = await httpFileResponse.Content.ReadAsByteArrayAsync();
            return httpFileBytes;
        }

        public async Task<bool> IsOutOfDate(string assemblyImage) {
            var assemblyVersionInfo = FileVersionInfo.GetVersionInfo(assemblyImage);
            if (!Version.TryParse(assemblyVersionInfo.ProductVersion, out var assemblyVersion)) {
                AnsiConsole.WriteLine("Failed to check identify current installed version.");
                return true;
            }

            var githubVersionString = await GetLatestVersion();
            var githubVersion = Version.Parse(githubVersionString.AsSpan().TrimStart("vV"));

            return assemblyVersion < githubVersion;
        }

        public async Task<string> GetLatestVersion() {
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            using var response = await _httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();

            var redirectLocation = response.RequestMessage!.RequestUri!.ToString();
            var versionString = redirectLocation.Substring(redirectLocation.LastIndexOf('/') + 1);

            return versionString;
        }
    }
}
