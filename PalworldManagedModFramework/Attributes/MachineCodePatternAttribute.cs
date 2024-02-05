namespace PalworldManagedModFramework.Attributes {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MachineCodePatternAttribute : Attribute {
        public MachineCodePatternAttribute(string pattern, OperandType patternType) {
            Pattern = pattern;
            PatternType = patternType;
        }

        public string Pattern { get; }
        public OperandType PatternType { get; }
    }

    public enum OperandType {
        DirectAddress_32,
        RelativeOffset_32
    }
}
