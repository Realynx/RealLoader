using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile {
    // TODO: make this class work
    public class InMemoryCompiler : ICodeCompiler {
        private readonly ILogger _logger;
        private readonly string _buildLocation;
        private readonly Dictionary<string, Dictionary<string, string>> _assemblySourceFiles = new();
        private readonly Dictionary<string, MetadataReference> _assemblyMetadata = new();
        private readonly Dictionary<string, ImmutableArray<byte>> _compiledAssemblies = new();

        public InMemoryCompiler(ILogger logger, string buildLocation) {
            _logger = logger;
            _buildLocation = buildLocation;
        }

        public void RegisterExistingAssembly(Assembly assembly) {
            var assemblyName = assembly.GetName();
            if (string.IsNullOrWhiteSpace(assemblyName.Name)) {
                throw new NullReferenceException($"{assemblyName.FullName} does not have a valid short name.");
            }

            ref var assemblyMetadata = ref CollectionsMarshal.GetValueRefOrAddDefault(_assemblyMetadata, assemblyName.Name, out var previouslyExisted);
            if (previouslyExisted) {
                _logger.Warning($"Tried to register {assemblyName.Name} more than once!");
                return;
            }

            if (!File.Exists(assembly.Location)) {
                throw new FileNotFoundException($"{assemblyName.Name} does not exist on disk at {assembly.Location}!");
            }

            assemblyMetadata = MetadataReference.CreateFromFile(assembly.Location);
        }

        public void AppendSolutionFile(StringBuilder code) {
            throw new NotImplementedException();
        }

        public void AppendProjectFile(StringBuilder code, string assemblyName) {
            throw new NotImplementedException();
        }

        public void AppendFile(StringBuilder code, string assemblyName, string nameSpace) {
            ref var assemblyFiles = ref CollectionsMarshal.GetValueRefOrAddDefault(_assemblySourceFiles, assemblyName, out var previouslyExisted);

            if (!previouslyExisted) {
                assemblyFiles = new Dictionary<string, string>();
            }

            if (!assemblyFiles!.TryAdd(nameSpace, code.ToString())) {
                throw new InvalidOperationException($"{nameSpace} already exists.");
            }
        }

        public void Compile() {
            foreach (var (assemblyName, _) in _assemblySourceFiles) {
                if (!_compiledAssemblies.ContainsKey(assemblyName)) {
                    _compiledAssemblies[assemblyName] = GetOrCompileAssembly(assemblyName);
                }
            }

            if (_assemblySourceFiles.Count != _compiledAssemblies.Count) {
                throw new Exception("Not all assemblies were compiled.");
            }

            foreach (var (assemblyName, assemblyBytes) in _compiledAssemblies) {
                var outputPath = Path.Combine(_buildLocation, $"{assemblyName}.dll");

                using var handle = File.OpenHandle(outputPath, FileMode.Create, FileAccess.Write, preallocationSize: assemblyBytes.Length);
                RandomAccess.Write(handle, assemblyBytes.AsSpan(), 0);

                _logger.Debug($"Wrote {assemblyName} to {outputPath}!");
            }
        }

        private IEnumerable<SyntaxTree> GetSyntaxTrees(string assemblyName) {
            if (!_assemblySourceFiles.TryGetValue(assemblyName, out var assemblyFiles)) {
                throw new InvalidOperationException($"{assemblyName} does not exist.");
            }

            foreach (var (_, sourceFileText) in assemblyFiles) {
                yield return CSharpSyntaxTree.ParseText(sourceFileText);
            }
        }

        private IEnumerable<MetadataReference> GetMetadataReferences(string compile, HashSet<string> toCompile) {
            foreach (var (assemblyName, _) in _assemblySourceFiles) {
                if (!_assemblyMetadata.TryGetValue(assemblyName, out var typeMetadata)) {
                    if (toCompile.Contains(assemblyName)) {
                        continue;
                    }

                    typeMetadata = MetadataReference.CreateFromImage(GetOrCompileAssembly(assemblyName, toCompile));
                    _assemblyMetadata[assemblyName] = typeMetadata;
                }
            }

            foreach (var (_, reference) in _assemblyMetadata) {
                yield return reference;
            }
        }

        private ImmutableArray<byte> GetOrCompileAssembly(string assemblyName, HashSet<string>? toCompile = null) {
            if (_compiledAssemblies.TryGetValue(assemblyName, out var compiledAssembly)) {
                return compiledAssembly;
            }

            toCompile ??= new HashSet<string>();
            toCompile.Add(assemblyName);

            var syntaxTrees = GetSyntaxTrees(assemblyName);
            var references = GetMetadataReferences(assemblyName, toCompile);
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, allowUnsafe: true);
            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, references, options);

            var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success) {
                DebugUtilities.WaitForDebuggerAttach();

                throw new InvalidOperationException($"Failed to emit compiled assembly for {assemblyName}: {string.Join(' ', emitResult.Diagnostics.Take(50))}");
            }

            if (!ms.TryGetBuffer(out var buffer)) {
                throw new InvalidOperationException("Failed to get stream buffer.");
            }

            var assemblyBytes = buffer.ToImmutableArray();
            _compiledAssemblies.Add(assemblyName, assemblyBytes);

            return assemblyBytes;
        }
    }
}