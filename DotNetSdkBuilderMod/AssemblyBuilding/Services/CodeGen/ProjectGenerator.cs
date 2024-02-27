using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

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

            const string PROJECT_TAG_VALUE = $"{SDK}{EQUALS}{QUOTE}{SDK_VALUE}{QUOTE}";
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
            codeBuilder.Append(TRUE);
            _xmlTagGenerator.GenerateCloseTag(codeBuilder, ALLOW_UNSAFE_BLOCKS);

            codeBuilder.AppendIndent(2);
            _xmlTagGenerator.GenerateOpenTag(codeBuilder, IMPLICIT_USINGS, newLine: false);
            codeBuilder.Append(TRUE);
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
                         var referenceValue = $"{INCLUDE}{EQUALS}{QUOTE}{reference}{QUOTE}";
                        codeBuilder.AppendIndent(2);
                        _xmlTagGenerator.GenerateSingleLineTag(codeBuilder, REFERENCE, referenceValue);
                    }
                    else {
                        var referenceValue = $"{INCLUDE}{EQUALS}{QUOTE}{DOT}{DOT}{BACK_SLASH}{reference}{BACK_SLASH}{reference}{DOT}{CSPROJ}{QUOTE}";
                        codeBuilder.AppendIndent(2);
                        _xmlTagGenerator.GenerateSingleLineTag(codeBuilder, PROJECT_REFERENCE, referenceValue);
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
            var copyItemsTagValue = $"{INCLUDE}=\"$(TargetDir)\\*.*\"";
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