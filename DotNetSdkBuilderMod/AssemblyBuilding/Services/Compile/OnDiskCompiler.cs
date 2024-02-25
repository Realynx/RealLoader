using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile {
    public class OnDiskCompiler : ICodeCompiler {
        private readonly ILogger _logger;
        private readonly string _buildLocation;
        private readonly List<string> _writtenFiles = new();

        public OnDiskCompiler(ILogger logger, string buildLocation) {
            _logger = logger;
            _buildLocation = buildLocation;
        }

        public void RegisterExistingAssembly(Assembly assembly) {
            throw new NotImplementedException();
        }

        public void AppendFile(StringBuilder code, string assemblyName, string nameSpace) {
            throw new NotImplementedException();
            var namespaceDirectories = Path.Combine(nameSpace.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            var directory = Path.Combine(_buildLocation, "source", namespaceDirectories);
            if (Directory.Exists(directory)) {
                Directory.Delete(directory, true);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Directory.CreateDirectory(directory);
            }
            else {
                Directory.CreateDirectory(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }

            var filePath = Path.Combine(directory, $"{nameSpace}.cs");
            if (File.Exists(filePath)) {
                throw new InvalidOperationException($"{filePath} already exists.");
            }

            using var sw = File.CreateText(filePath);
            sw.Write(code);

            _writtenFiles.Add(filePath);
            // _logger.Debug($"Wrote code file {filePath}");
        }

        public void Compile() {
            _writtenFiles.Clear();
        }

        public bool TryGetCompiledAssembly(string assemblyName, out ImmutableArray<byte> assemblyBytes) {
            throw new NotImplementedException();
        }

        private IEnumerable<SyntaxTree> GetSyntaxTrees() {
            foreach (var file in _writtenFiles) {
                yield return CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file);
            }
        }
    }
}