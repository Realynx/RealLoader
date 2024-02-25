namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public class FullTypePathAttribute : Attribute {
        public FullTypePathAttribute(string path) {
            Path = path;
        }

        public string Path { get; }
    }
}