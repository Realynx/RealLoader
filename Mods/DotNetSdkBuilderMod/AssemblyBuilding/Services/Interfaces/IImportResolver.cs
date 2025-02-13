using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

public interface IImportResolver {
    void ApplyImports(CodeGenNamespaceNode currentNode, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces);
}