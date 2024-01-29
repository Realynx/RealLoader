
using System.Diagnostics;

using PalworldManagedModFramework.Services.Logging;

namespace PalworldManagedModFramework.Services {
    internal class GameExplorer {
        private readonly ILogger _logger;

        public GameExplorer(ILogger logger) {
            _logger = logger;
        }

        internal unsafe void Entry() {
            _logger.Info("Begin memory explorer...");

            var palWorldProcess = Process.GetCurrentProcess();
            ProcessModule palWorldGameModule = null;
            foreach (ProcessModule processModule in palWorldProcess.Modules) {
                if (!processModule.ModuleName.Contains("pal", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                if (processModule.ModuleName.Equals("Game-Palworld-Win64-Shipping.exe", StringComparison.OrdinalIgnoreCase)) {
                    palWorldGameModule = processModule;
                }

                _logger.Info(@$"Symbol Name: {processModule.ModuleName}
Base Address: 0x{processModule.BaseAddress:X},
Symbol Length: {processModule.ModuleMemorySize:X}
File: {processModule.FileName}
");


            }
        }
    }
}
