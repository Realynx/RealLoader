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

        public static StringBuilder RemoveLine(this StringBuilder sb) {
            return sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }
    }
}
