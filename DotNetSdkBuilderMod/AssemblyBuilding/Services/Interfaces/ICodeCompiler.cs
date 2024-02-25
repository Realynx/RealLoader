using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeCompiler {
        void RegisterExistingAssembly(Assembly assembly);
        void AppendFile(StringBuilder code, string assemblyName, string nameSpace);
        void Compile();
        bool TryGetCompiledAssembly(string assemblyName, out ImmutableArray<byte> assemblyBytes);
    }
}