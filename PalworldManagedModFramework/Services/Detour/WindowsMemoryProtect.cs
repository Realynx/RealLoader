using Microsoft.Extensions.Logging;

namespace PalworldManagedModFramework.Services.Detour {
    public class WindowsMemoryProtect {
        private readonly ILogger _logger;

        public WindowsMemoryProtect(ILogger logger) {
            _logger = logger;
        }
    }
}
