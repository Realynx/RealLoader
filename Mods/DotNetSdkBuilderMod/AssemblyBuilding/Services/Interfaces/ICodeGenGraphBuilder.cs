using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenGraphBuilder {
        CodeGenAssemblyNode[] BuildAssemblyGraphs(ClassNode rootNode);
    }
}