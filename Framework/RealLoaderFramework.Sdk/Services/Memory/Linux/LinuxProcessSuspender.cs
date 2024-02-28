using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;

namespace RealLoaderFramework.Sdk.Services.Memory.Linux {
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
