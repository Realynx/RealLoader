namespace PalworldManagedModFramework.Sdk.Attributes {
    public class HookAttribute : Attribute {
        public HookAttribute(string pattern) {
            Pattern = pattern;
        }

        public string Pattern { get; }
    }
}
