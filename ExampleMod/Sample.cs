using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;

namespace ExampleMod {
    [PalworldMod("Sample", "poofyfox", ".poofyfox", "1.0.0")]
    public class Sample : IPalworldMod {
        private CancellationToken _cancellationToken;

        public void Load(CancellationToken cancellationToken) {
            _cancellationToken = cancellationToken;

            Console.WriteLine("Hello World!");
        }

        public void Unload() {
            Console.WriteLine("Unloading");
        }
    }
}
