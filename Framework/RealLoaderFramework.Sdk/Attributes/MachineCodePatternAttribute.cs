namespace RealLoaderFramework.Sdk.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class MachineCodePatternAttribute : Attribute {

        public MachineCodePatternAttribute(string pattern, PatternType patternType) {
            Pattern = pattern;
            PatternType = patternType;
        }

        public string Pattern { get; }
        public PatternType PatternType { get; }
    }

    public enum PatternType {
        Detour,
        Function,
        DirectAddress_32,
        IP_RelativeOffset_32,
        Base_RelativeOffset_32
    }
}
