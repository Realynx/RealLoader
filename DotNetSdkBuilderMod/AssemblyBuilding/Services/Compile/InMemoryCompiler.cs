using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile {
    public class InMemoryCompiler : ICodeCompiler {
        private readonly ILogger _logger;
        private readonly string _buildLocation;
        private readonly Dictionary<string, string> _sourceFiles = new();

        public InMemoryCompiler(ILogger logger, string buildLocation) {
            _logger = logger;
            _buildLocation = buildLocation;
        }

        public void AppendFile(StringBuilder code, string nameSpace) {
            if (!_sourceFiles.TryAdd(nameSpace, code.ToString())) {
                throw new InvalidOperationException($"{nameSpace} already exists.");
            }
        }

        public void Compile(string assemblyName) {
            CSharpCompilation.Create(assemblyName, GetSyntaxTrees());

            _sourceFiles.Clear();
        }

        private IEnumerable<SyntaxTree> GetSyntaxTrees() {
            foreach (var (_, sourceFileText) in _sourceFiles) {
                yield return CSharpSyntaxTree.ParseText(sourceFileText);
            }
        }
    }
}
