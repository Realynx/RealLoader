using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IUObjectInteropExtensionsBuilder {
        void PopulateNamespaceNode(CodeGenNamespaceNode extensionsNamespaceNode, IReadOnlyList<int> functionArgCounts);
        CodeGenNamespaceNode GetScaffoldNamespaceNode();
    }
}