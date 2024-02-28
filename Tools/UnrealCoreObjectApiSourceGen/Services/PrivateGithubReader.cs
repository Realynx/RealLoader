using Microsoft.Extensions.Logging;

using UnrealCoreObjectApiSourceGen.Services.Interfaces;

namespace UnrealCoreObjectApiSourceGen.Services {
    public class PrivateGithubReader : IPrivateGithubReader {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public PrivateGithubReader(ILogger logger, HttpClient httpClient) {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<string> DownloadHeaderFile(string engineVersion, string headerFile) {
            var query = $"{engineVersion}/Engine/Source/Runtime/CoreUObject/Public/UObject/{headerFile}";
            _logger.LogInformation($"Downloading, {query}");

            var httpResonse = await _httpClient.GetAsync(query);
            if (!httpResonse.IsSuccessStatusCode) {
                var errorMessage = $"Could not download the source file: {httpResonse.RequestMessage?.RequestUri}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var nativeSource = await httpResonse.Content.ReadAsStringAsync();
            return nativeSource;
        }
    }
}
