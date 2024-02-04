using System.Runtime.CompilerServices;
using System.Security.AccessControl;

using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services;

namespace ExampleServerMod {
    [PalworldMod("Sample", "poofyfox", ".poofyfox", "1.0.0", serverMod: true)]
    public class ExampleServerMod : IPalworldMod {
        private ILogger _logger;
        private CancellationToken _cancellationToken;

        public void Load(CancellationToken cancellationToken, ILogger logger) {
            _logger = logger;
            _cancellationToken = cancellationToken;

            _logger.Info($"Beginning byte code scans");

            //// Windows Bytecode Pattern
            //var gWorld = SequenceScanner.SingleSequenceScan("49 ?? ?? ?? 48 ?? ?? ?? ?? ?? ?? 49 ?? ?? ?? f6");
            //var gobalWorldAddress = gWorld + 5;
            //_logger.Info($"Found Global World Instance: 0x{gobalWorldAddress:X}");

            //var gObjects = SequenceScanner.SingleSequenceScan("48 8B 05 ?? ?? ?? ?? 48 8B 0C C8 4C 8D 04 D1 EB 03");
            //_logger.Info($"Found Global Objects Instance: 0x{gObjects:X}");

            //var fNames = SequenceScanner.SingleSequenceScan("48 8D 05 ?? ?? ?? ?? EB 13 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? C6 05 ?? ?? ?? ?? ?? 0F 10");
            //_logger.Info($"Found F Names Instance: 0x{fNames:X}");

            // compiled
            Console.WriteLine("d");

            //var fNames = SequenceScanner.SingleSequenceScan("48 8D 05 ?? ?? ?? ?? EB 13 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? C6 05 ?? ?? ?? ?? ?? 0F 10");
            //_logger.Info($"Found F Names Instance: 0x{fNames:X}");
        }

        public void Unload() {

        }
    }
}
