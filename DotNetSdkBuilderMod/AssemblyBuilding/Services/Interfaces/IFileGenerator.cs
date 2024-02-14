using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IFileGenerator {
        void GenerateFile(StringBuilder codeBuilder, ClassNode classNode, string nameSpace);
    }
}