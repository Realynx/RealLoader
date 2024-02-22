namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public class FullTypePathAttribute {
        public FullTypePathAttribute(string path) {
            Path = path;
        }

        public string Path { get; }
    }
}