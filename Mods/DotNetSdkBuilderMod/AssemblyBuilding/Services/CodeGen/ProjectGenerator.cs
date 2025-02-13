using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ProjectGenerator : IProjectGenerator {
        private readonly ILogger _logger;
        private readonly IXmlTagGenerator _xmlTagGenerator;

        public ProjectGenerator(ILogger logger, IXmlTagGenerator xmlTagGenerator) {
            _logger = logger;
            _xmlTagGenerator = xmlTagGenerator;
        }

        public StringBuilder GenerateProject(CodeGenAssemblyNode assemblyNode) {
            var codeBuilder = new StringBuilder();

            const string PROJECT_TAG_VALUE = "Sdk=\"Microsoft.NET.Sdk\"";
            _xmlTagGenerator.GenerateOpenTag(codeBuilder, PROJECT, PROJECT_TAG_VALUE);
            codeBuilder.AppendLine();

            // PropertyGroup
            codeBuilder.AppendIndent(1);
            _xmlTagGenerator.GenerateOpenTag(codeBuilder, PROPERTY_GROUP);

            codeBuilder.AppendIndent(2);
            _xmlTagGenerator.GenerateOpenTag(codeBuilder, TARGET_FRAMEWORK, newLine: false);
            codeBuilder.Append(TARGET_FRAMEWORK_VALUE);
            _xmlTagGenerator.GenerateCloseTag(codeBuilder, TARGET_FRAMEWORK);

            codeBuilder.AppendIndent(2);
            _xmlTagGenerator.GenerateOpenTag(codeBuilder, ALLOW_UNSAFE_BLOCKS, newLine: false);
            codeBuilder.Append("true");
            _xmlTagGenerator.GenerateCloseTag(codeBuilder, ALLOW_UNSAFE_BLOCKS);

            codeBuilder.AppendIndent(2);
            _xmlTagGenerator.GenerateOpenTag(codeBuilder, IMPLICIT_USINGS, newLine: false);
            codeBuilder.Append("true");
            _xmlTagGenerator.GenerateCloseTag(codeBuilder, IMPLICIT_USINGS);

            codeBuilder.AppendIndent(1);
            _xmlTagGenerator.GenerateCloseTag(codeBuilder, PROPERTY_GROUP);

            codeBuilder.AppendLine();

            // ItemGroup
            codeBuilder.AppendIndent(1);
            _xmlTagGenerator.GenerateOpenTag(codeBuilder, ITEM_GROUP);

            if (assemblyNode.references is not null) {
                foreach (var reference in assemblyNode.references) {
                    if (reference.EndsWith(".dll")) {
                         var referenceValue = $"Include=\"{reference}\"";
                        codeBuilder.AppendIndent(2);
                        _xmlTagGenerator.GenerateSingleLineTag(codeBuilder, "Reference", referenceValue);
                    }
                    else {
                        var referenceValue = $"Include=\"..\\{reference}\\{reference}.csproj\"";
                        codeBuilder.AppendIndent(2);
                        _xmlTagGenerator.GenerateSingleLineTag(codeBuilder, "ProjectReference", referenceValue);
                    }
                }
            }
            codeBuilder.AppendIndent(1);
            _xmlTagGenerator.GenerateCloseTag(codeBuilder, ITEM_GROUP);

            codeBuilder.AppendLine();

            var targetTagValue = $"Name=\"PostBuild\" AfterTargets=\"PostBuildEvents\" Condition=\"'$({BUILD_OUTPUT_ENVIRONMENT_VARIABLE})' != ''\"";
            codeBuilder.AppendIndent(1);
            _xmlTagGenerator.GenerateOpenTag(codeBuilder, TARGET, targetTagValue);
            codeBuilder.AppendIndent(2);
            _xmlTagGenerator.GenerateOpenTag(codeBuilder,  ITEM_GROUP);
            var copyItemsTagValue = $"Include=\"$(TargetDir)\\*.*\"";
            codeBuilder.AppendIndent(3);
            _xmlTagGenerator.GenerateSingleLineTag(codeBuilder,  COPY_ITEMS, copyItemsTagValue);
            codeBuilder.AppendIndent(2);
            _xmlTagGenerator.GenerateCloseTag(codeBuilder,  ITEM_GROUP);

            var copyTagValue = $"SourceFiles=\"@({COPY_ITEMS})\" DestinationFolder=\"$({BUILD_OUTPUT_ENVIRONMENT_VARIABLE})\"";
            codeBuilder.AppendIndent(2);
            _xmlTagGenerator.GenerateSingleLineTag(codeBuilder,  COPY, copyTagValue);

            codeBuilder.AppendIndent(1);
            _xmlTagGenerator.GenerateCloseTag(codeBuilder, TARGET);

            codeBuilder.AppendLine();

            _xmlTagGenerator.GenerateCloseTag(codeBuilder, PROJECT);

            return codeBuilder;
        }
    }
}