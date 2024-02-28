using Microsoft.Extensions.Configuration;

namespace UnrealCoreObjectApiSourceGen.Models {
    public class GithubConfig {
        public GithubConfig(IConfiguration configuration) {
            configuration.GetSection(nameof(GithubConfig)).Bind(this);
        }

        public string AccessToken { get; set; }
    }
}
