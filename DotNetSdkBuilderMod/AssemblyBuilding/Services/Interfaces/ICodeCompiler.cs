using System.Reflection;
using System.Text;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeCompiler {
        void RegisterExistingAssembly(Assembly assembly);
        void AppendSolutionFile(StringBuilder code);
        void AppendProjectFile(StringBuilder code, string assemblyName);
        void AppendFile(StringBuilder code, string assemblyName, string nameSpace);
        void Compile();
    }
}