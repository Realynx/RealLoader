using System.Globalization;
using System.Text.RegularExpressions;

namespace PalworldManagedModFramework.PalWorldSdk.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class MachineCodePatternAttribute : Attribute {

        public MachineCodePatternAttribute(string pattern, OperandType patternType) {
            Pattern = pattern;
            PatternType = patternType;
        }

        public string Pattern { get; }
        public OperandType PatternType { get; }
    }

    public enum OperandType {
        Function,
        DirectAddress_32,
        IP_RelativeOffset_32,
        Base_RelativeOffset_32
    }
}
