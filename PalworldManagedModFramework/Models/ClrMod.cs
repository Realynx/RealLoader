using System.Reflection;

using PalworldManagedModFramework.PalWorldSdk.Attributes;

namespace PalworldManagedModFramework.Models {
    public class ClrMod {
        public string AssemblyPath { get; init; }
        public Assembly Assembly { get; set; }
        public PalworldModAttribute PalworldModAttribute { get; init; }
    }
}
