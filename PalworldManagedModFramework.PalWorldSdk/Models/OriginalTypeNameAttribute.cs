namespace PalworldManagedModFramework.Sdk.Models {
    [AttributeUsage(AttributeTargets.Class)]
    public class OriginalTypeNameAttribute : Attribute {
        public OriginalTypeNameAttribute(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}