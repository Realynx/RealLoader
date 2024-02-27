namespace PalworldManagedModFramework.Sdk.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class HookEngineEventAttribute : Attribute {
        public HookEngineEventAttribute(string fullyQualifiedName) {
            FullyQualifiedName = fullyQualifiedName;
        }

        public string FullyQualifiedName { get; }
    }
}
