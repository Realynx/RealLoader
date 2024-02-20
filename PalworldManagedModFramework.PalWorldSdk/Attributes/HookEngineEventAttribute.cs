namespace PalworldManagedModFramework.Sdk.Attributes {
    public class HookEngineEventAttribute : Attribute {
        public HookEngineEventAttribute(string fullyQualifiedName) {
            FullyQualifiedName = fullyQualifiedName;
        }

        public string FullyQualifiedName { get; }
    }
}
