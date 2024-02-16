using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

public interface IImportResolver {
    void ApplyImports(CodeGenNamespaceNode current, Dictionary<string, string> classNamespaces);
}