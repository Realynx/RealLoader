using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ISolutionGenerator {
        StringBuilder GenerateSolution(CodeGenAssemblyNode[] assemblyNodes);
    }
}