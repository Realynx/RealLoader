using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IProjectGenerator {
        StringBuilder GenerateProject(CodeGenAssemblyNode assemblyNode);
    }
}