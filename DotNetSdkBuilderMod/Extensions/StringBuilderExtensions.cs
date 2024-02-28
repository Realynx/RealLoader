using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen;

namespace DotNetSdkBuilderMod.Extensions {
    public static class StringBuilderExtensions {
        public static StringBuilder AppendIndentedLine(this StringBuilder sb, string str, int indentSize) {
            return sb
                .Append(CodeGenConstants.INDENT, indentSize * CodeGenConstants.INDENT_SIZE)
                .AppendLine(str);
        }

        public static StringBuilder AppendIndented(this StringBuilder sb, string str, int indentSize) {
            return sb
                .Append(CodeGenConstants.INDENT, indentSize * CodeGenConstants.INDENT_SIZE)
                .Append(str);
        }

        public static StringBuilder AppendIndent(this StringBuilder sb, int indentSize) {
            return sb
                .Append(CodeGenConstants.INDENT, indentSize * CodeGenConstants.INDENT_SIZE);
        }

        public static StringBuilder AppendTabIndent(this StringBuilder sb, int tabCount) {
            return sb
                .Append(CodeGenConstants.TAB, tabCount);
        }

        public static StringBuilder RemoveNewLine(this StringBuilder sb) {
            if (sb.Length < Environment.NewLine.Length) {
                return sb;
            }

            return sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }

        public static StringBuilder TrimStart(this StringBuilder sb, char character) {
            var removeLength = 0;
            while (sb.Length > removeLength + 1 && sb[removeLength] == character) {
                removeLength++;
            }

            return sb.Remove(0, removeLength);
        }
    }
}