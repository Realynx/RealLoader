using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

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

            codeBuilder.AppendLine("Global");

            // Solution build config
            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine("GlobalSection(SolutionConfigurationPlatforms) = preSolution");

            var buildConfigs = new[] { "Debug|Any CPU", "Release|Any CPU" };
            foreach (var config in buildConfigs) {
                codeBuilder.AppendTabIndent(2);
                codeBuilder.AppendLine($"{config} = {config}");
            }

            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine("EndGlobalSection");

            // Project build configs
            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine("GlobalSection(ProjectConfigurationPlatforms) = postSolution");

            for (var i = 0; i < assemblyNodes.Length; i++) {
                foreach (var config in buildConfigs) {
                    codeBuilder.AppendTabIndent(2);
                    codeBuilder.AppendLine($$"""{{{projectGuids[i]}}}.{{config}}.ActiveCfg = ""{{{config}}}""");

                    codeBuilder.AppendTabIndent(2);
                    codeBuilder.AppendLine($$"""{{{projectGuids[i]}}}.{{config}}.Build = ""{{{config}}}""");
                }
            }

            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine("EndGlobalSection");

            // Solution properties
            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine("GlobalSection(SolutionProperties) = preSolution");

            codeBuilder.AppendTabIndent(2);
            codeBuilder.AppendLine("HideSolutionNode = FALSE");

            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine("EndGlobalSection");

            // Solution guid
            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine("GlobalSection(ExtensibilityGlobals) = postSolution");

            codeBuilder.AppendTabIndent(2);
            codeBuilder.AppendLine($$"""SolutionGuid = {{{Guid.NewGuid().ToString().ToUpper()}}}""");

            codeBuilder.AppendTabIndent(1);
            codeBuilder.AppendLine("EndGlobalSection");

            codeBuilder.Append("EndGlobal");

            return codeBuilder;
        }

        private void GenerateProjectSection(StringBuilder codeBuilder, string projectTypeGuid, CodeGenAssemblyNode assemblyNode, string projectGuid) {
            codeBuilder.AppendLine(
                $$"""
                  Project("{{{projectTypeGuid}}}") = "{{assemblyNode.name}}", "{{assemblyNode.name}}\{{assemblyNode.name}}.csproj", "{{{projectGuid}}}"
                  """
            );

            codeBuilder.AppendLine("EndProject");
        }
    }
}