using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux {
    public class LinuxProcessSuspender : IProcessSuspender {
        private readonly ILogger _logger;

        public LinuxProcessSuspender(ILogger logger) {
            _logger = logger;
        }

        /// <summary>
        /// Yea... good luck
        /// </summary>
        public void PauseSelf() {
            _logger.Debug($"{nameof(PauseSelf)} Was called on linux, cannot pause threads as a child thread!");
        }

        public void ResumeSelf() {

        }
    }
}
