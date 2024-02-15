using System.Text;

namespace DotNetSdkBuilderMod.Extensions {
    public static class StringBuilderExtensions {
        private const int INDENT_MULTIPLIER = 4;

        public static StringBuilder AppendIndentedLine(this StringBuilder sb, string str, int indentSize) {
            return sb
                .Append(' ', indentSize * INDENT_MULTIPLIER)
                .AppendLine(str);
        }

        public static StringBuilder AppendIndented(this StringBuilder sb, string str, int indentSize) {
            return sb
                .Append(' ', indentSize * INDENT_MULTIPLIER)
                .Append(str);
        }

        public static StringBuilder RemoveLine(this StringBuilder sb) {
            return sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }
    }
}
