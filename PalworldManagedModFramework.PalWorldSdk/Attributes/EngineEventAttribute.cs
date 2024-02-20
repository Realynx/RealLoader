using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PalworldManagedModFramework.Sdk.Attributes {
    public class EngineEventAttribute : Attribute {
        public EngineEventAttribute([StringSyntax(StringSyntaxAttribute.Regex)] string eventMask) {
            EventMask = new Regex(eventMask, RegexOptions.Compiled);
        }

        public Regex EventMask { get; }
    }
}
