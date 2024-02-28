using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class XmlTagGenerator : IXmlTagGenerator {
        public void GenerateOpenTag(StringBuilder codeBuilder, string tagName, string? tagValue = null, bool newLine = true) {
            codeBuilder.Append(OPEN_ANGLE_BRACKET);
            codeBuilder.Append(tagName);

            if (!string.IsNullOrWhiteSpace(tagValue)) {
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.Append(tagValue);
            }

            codeBuilder.Append(CLOSED_ANGLE_BRACKET);

            if (newLine) {
                codeBuilder.AppendLine();
            }
        }

        public void GenerateCloseTag(StringBuilder codeBuilder, string tagName, bool newLine = true) {
            codeBuilder.Append(OPEN_ANGLE_BRACKET);
            codeBuilder.Append(FORWARD_SLASH);
            codeBuilder.Append(tagName);
            codeBuilder.Append(CLOSED_ANGLE_BRACKET);

            if (newLine) {
                codeBuilder.AppendLine();
            }
        }

        public void GenerateSingleLineTag(StringBuilder codeBuilder, string tagName, string? tagValue = null) {
            codeBuilder.Append(OPEN_ANGLE_BRACKET);
            codeBuilder.Append(tagName);

            if (!string.IsNullOrWhiteSpace(tagValue)) {
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.Append(tagValue);
            }

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(FORWARD_SLASH);
            codeBuilder.AppendLine(CLOSED_ANGLE_BRACKET);
        }
    }
}