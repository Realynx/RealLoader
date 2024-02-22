using System.Text;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeCompiler {
        void AppendFile(StringBuilder code, string nameSpace);
        void Compile();
    }
}