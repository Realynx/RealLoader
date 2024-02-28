using Microsoft.Extensions.Configuration;

namespace RealLoaderFramework.Models.Config {
    public class ModLoaderConfig {
        public ModLoaderConfig(IConfiguration configuration) {
            configuration.GetSection(nameof(ModLoaderConfig)).Bind(this);
        }

        public string ModFolder { get; set; }
        public bool EnableMods { get; set; }
        public bool EnableModWhiteList { get; set; }
        public string[] ModWhiteList { get; set; }
    }
}
