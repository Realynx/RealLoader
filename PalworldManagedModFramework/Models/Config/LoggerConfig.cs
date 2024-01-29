using Microsoft.Extensions.Configuration;

namespace PalworldManagedModFramework.Models.Config {
    internal class LoggerConfig {
        public LoggerConfig(IConfiguration configuration) {
            configuration.GetSection(nameof(LoggerConfig)).Bind(this);
        }

        public string LogFile { get; set; }
        public bool DebugLogs { get; set; }
        public bool WriteFile { get; set; }
    }
}
