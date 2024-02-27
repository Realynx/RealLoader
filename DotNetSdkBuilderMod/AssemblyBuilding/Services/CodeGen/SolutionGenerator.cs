using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class SolutionGenerator : ISolutionGenerator {
        private readonly ILogger _logger;

        public SolutionGenerator(ILogger logger) {
            _logger = logger;
        }

        public StringBuilder GenerateSolution(CodeGenAssemblyNode[] assemblyNodes) {
            var codeBuilder = new StringBuilder();

            // Header
            codeBuilder.AppendLine();
            codeBuilder.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            codeBuilder.AppendLine("# Visual Studio Version 17");
            codeBuilder.AppendLine("VisualStudioVersion = 17.8.34316.72");
            codeBuilder.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

            // Projects
            var projectTypeGuid = Guid.NewGuid().ToString().ToUpper();
            var projectGuids = assemblyNodes.Select(_ => Guid.NewGuid().ToString().ToUpper()).ToArray();
            for (var i = 0; i < assemblyNodes.Length; i++) {
                GenerateProjectSection(codeBuilder, projectTypeGuid, assemblyNodes[i], projectGuids[i]);
            }

            codeBuilder.AppendLine(GLOBAL);

            // Solution build config
            codeBuilder.AppendTabIndent(1);
            codeBuilder.Append(GLOBAL_SECTION);
            codeBuilder.Append(OPEN_ROUND_BRACKET);
            codeBuilder.Append(SOLUTION_CONFIGURATION_PLATFORMS);
            codeBuilder.Append(CLOSED_ROUND_BRACKET);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(EQUALS);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(PRE_SOLUTION);

            var buildConfigs = new[] { "Debug|Any CPU", "Release|Any CPU" };
            foreach (var config in buildConfigs) {
                codeBuilder.AppendTabIndent(2);
                codeBuilder.Append(config);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.Append(EQUALS);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.AppendLine(config);
            }

            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine(END_GLOBAL_SECTION);

            // Project build configs
            codeBuilder.AppendTabIndent(1);
            codeBuilder.Append(GLOBAL_SECTION);
            codeBuilder.Append(OPEN_ROUND_BRACKET);
            codeBuilder.Append(PROJECT_CONFIGURATION_PLATFORMS);
            codeBuilder.Append(CLOSED_ROUND_BRACKET);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(EQUALS);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(POST_SOLUTION);

            for (var i = 0; i < assemblyNodes.Length; i++) {
                foreach (var config in buildConfigs) {
                    codeBuilder.AppendTabIndent(2);
                    codeBuilder.Append(OPEN_CURLY_BRACKET);
                    codeBuilder.Append(projectGuids[i]);
                    codeBuilder.Append(CLOSED_CURLY_BRACKET);
                    codeBuilder.Append(DOT);
                    codeBuilder.Append(config);
                    codeBuilder.Append(DOT);
                    codeBuilder.Append("ActiveCfg");
                    codeBuilder.Append(WHITE_SPACE);
                    codeBuilder.Append(EQUALS);
                    codeBuilder.Append(WHITE_SPACE);
                    codeBuilder.AppendLine(config);

                    codeBuilder.AppendTabIndent(2);
                    codeBuilder.Append(OPEN_CURLY_BRACKET);
                    codeBuilder.Append(projectGuids[i]);
                    codeBuilder.Append(CLOSED_CURLY_BRACKET);
                    codeBuilder.Append(DOT);
                    codeBuilder.Append(config);
                    codeBuilder.Append(DOT);
                    codeBuilder.Append("Build");
                    codeBuilder.Append(DOT);
                    codeBuilder.Append(0);
                    codeBuilder.Append(WHITE_SPACE);
                    codeBuilder.Append(EQUALS);
                    codeBuilder.Append(WHITE_SPACE);
                    codeBuilder.AppendLine(config);
                }
            }

            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine(END_GLOBAL_SECTION);

            // Solution properties
            codeBuilder.AppendTabIndent(1);
            codeBuilder.Append(GLOBAL_SECTION);
            codeBuilder.Append(OPEN_ROUND_BRACKET);
            codeBuilder.Append("SolutionProperties");
            codeBuilder.Append(CLOSED_ROUND_BRACKET);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(EQUALS);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(PRE_SOLUTION);

            codeBuilder.AppendTabIndent(2);
            codeBuilder.Append("HideSolutionNode");
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(EQUALS);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine("FALSE");

            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine(END_GLOBAL_SECTION);

            // Solution guid
            codeBuilder.AppendTabIndent(1);
            codeBuilder.Append(GLOBAL_SECTION);
            codeBuilder.Append(OPEN_ROUND_BRACKET);
            codeBuilder.Append("ExtensibilityGlobals");
            codeBuilder.Append(CLOSED_ROUND_BRACKET);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(EQUALS);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(POST_SOLUTION);

            codeBuilder.AppendTabIndent(2);
            codeBuilder.Append("SolutionGuid");
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(EQUALS);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(OPEN_CURLY_BRACKET);
            codeBuilder.Append(Guid.NewGuid().ToString().ToUpper());
            codeBuilder.AppendLine(CLOSED_CURLY_BRACKET);

            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine(END_GLOBAL_SECTION);

            codeBuilder.Append(END_GLOBAL);

            return codeBuilder;
        }

        private void GenerateProjectSection(StringBuilder codeBuilder, string projectTypeGuid, CodeGenAssemblyNode assemblyNode, string projectGuid) {
            codeBuilder.Append(PROJECT);
            codeBuilder.Append(OPEN_ROUND_BRACKET);
            codeBuilder.Append(QUOTE);
            codeBuilder.Append(OPEN_CURLY_BRACKET);
            codeBuilder.Append(projectTypeGuid);
            codeBuilder.Append(CLOSED_CURLY_BRACKET);
            codeBuilder.Append(QUOTE);
            codeBuilder.Append(CLOSED_ROUND_BRACKET);

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(EQUALS);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(QUOTE);
            codeBuilder.Append(assemblyNode.name);
            codeBuilder.Append(QUOTE);
            codeBuilder.Append(COMMA);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(QUOTE);
            codeBuilder.Append($"{assemblyNode.name}{BACK_SLASH}{assemblyNode.name}{DOT}{CSPROJ}");
            codeBuilder.Append(QUOTE);
            codeBuilder.Append(COMMA);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(QUOTE);
            codeBuilder.Append(OPEN_CURLY_BRACKET);
            codeBuilder.Append(projectGuid);
            codeBuilder.Append(CLOSED_CURLY_BRACKET);
            codeBuilder.AppendLine(QUOTE);

            codeBuilder.AppendLine(END_PROJECT);
        }
    }
}