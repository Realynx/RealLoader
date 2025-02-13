using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class XmlTagGenerator : IXmlTagGenerator {
        public void GenerateOpenTag(StringBuilder codeBuilder, string tagName, string? tagValue = null, bool newLine = true) {
            codeBuilder.Append($"<{tagName}");

            if (!string.IsNullOrWhiteSpace(tagValue)) {
                codeBuilder.Append($" {tagValue}");
            }

            codeBuilder.Append('>');

            if (newLine) {
                codeBuilder.AppendLine();
            }
        }

        public void GenerateCloseTag(StringBuilder codeBuilder, string tagName, bool newLine = true) {
            codeBuilder.Append($"</{tagName}>");

            if (newLine) {
                codeBuilder.AppendLine();
            }
        }

        public void GenerateSingleLineTag(StringBuilder codeBuilder, string tagName, string? tagValue = null) {
            codeBuilder.Append($"<{tagName}");

            if (!string.IsNullOrWhiteSpace(tagValue)) {
                codeBuilder.Append($" {tagValue}");
            }

            codeBuilder.AppendLine(" />");
        }
    }
}