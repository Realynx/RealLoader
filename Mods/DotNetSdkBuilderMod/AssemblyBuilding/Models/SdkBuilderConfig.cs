using Microsoft.Extensions.Configuration;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public class SdkBuilderConfig {
        public SdkBuilderConfig(IConfiguration configuration) {
            configuration.GetSection(nameof(SdkBuilderConfig))
                .Bind(this);
        }

        public bool BuildInMemory { get; set; }
        public string BuildLocation { get; set; }
        public bool DisplayCompilerOutput { get; set; }
    }
}
