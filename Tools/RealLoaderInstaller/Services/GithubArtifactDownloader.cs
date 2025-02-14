﻿using System.Diagnostics;
using System.IO.Compression;

using RealLoaderInstaller.Models;
using RealLoaderInstaller.Services.Interfaces;

using Spectre.Console;

namespace RealLoaderInstaller.Services {
    public class GithubArtifactDownloader : IGithubArtifactDownloader {
        internal const string FRAMEWORK_ZIP = "ManagedModFramework.zip";
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

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, githubFileName);
            var httpFileResponse = await _httpClient.SendAsync(httpRequest);

            if (!httpFileResponse.IsSuccessStatusCode) {
                AnsiConsole.WriteLine($"Failed to download '{githubFileName}'...");
            }

            var httpFileBytes = await httpFileResponse.Content.ReadAsByteArrayAsync();
            return httpFileBytes;
        }

        public async Task UnzipFrameworkPackage(string extractPath) {
            var zipFileBytes = await DownloadGithubReleaseAsync(FRAMEWORK_ZIP);
            await File.WriteAllBytesAsync(FRAMEWORK_ZIP, zipFileBytes);

            var tempdir = Directory.CreateTempSubdirectory();

            using var archive = ZipFile.OpenRead(FRAMEWORK_ZIP);
            archive.ExtractToDirectory(tempdir.FullName, true);

            Directory.Delete(extractPath, true);
            Directory.Move(Path.Combine(tempDir.FullName, "Release", "net9.0"), extractPath);

            tempdir.Delete(true);
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
