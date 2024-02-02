namespace PalworldManagedModFramework.PalWorldSdk.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SequencePatternAttribute : Attribute {
        public SequencePatternAttribute(string pattern, int offset) {
            Pattern = pattern;
            Offset = offset;
        }

        public string Pattern { get; }
        public int Offset { get; }
    }
}
