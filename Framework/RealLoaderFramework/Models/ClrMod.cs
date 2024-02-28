using System.Reflection;

using RealLoaderFramework.Sdk.Attributes;

namespace RealLoaderFramework.Models {
    public class ClrMod {
        public string AssemblyPath { get; init; }
        public Assembly Assembly { get; set; }
        public RealLoaderModAttribute ModAttribute { get; init; }
    }
}
