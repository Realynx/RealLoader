namespace RealLoaderFramework.Sdk.Models {
    [AttributeUsage(AttributeTargets.Class)]
    public class FullTypePathAttribute : Attribute {
        public FullTypePathAttribute(string path) {
            Path = path;
        }

        public string Path { get; }
    }
}