namespace RealLoaderFramework.Sdk.Attributes {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class OriginalMemberNameAttribute : Attribute {
        public OriginalMemberNameAttribute(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}