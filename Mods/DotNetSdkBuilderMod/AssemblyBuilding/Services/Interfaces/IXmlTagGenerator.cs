using System.Text;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IXmlTagGenerator {
        void GenerateOpenTag(StringBuilder codeBuilder, string tagName, string? tagValue = null, bool newLine = true);
        void GenerateCloseTag(StringBuilder codeBuilder, string tagName, bool newLine = true);
        void GenerateSingleLineTag(StringBuilder codeBuilder, string tagName, string? tagValue = null);
    }
}