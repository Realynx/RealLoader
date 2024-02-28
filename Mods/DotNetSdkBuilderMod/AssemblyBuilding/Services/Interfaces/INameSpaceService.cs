using System.Reflection;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface INameSpaceService {
        void GetUniqueNamespaces(ClassNode currentNode, HashSet<string> namespaces);
        CodeGenNamespaceNode BuildNamespaceTree(string[] namespaceNames, string previousNamespace, CodeGenNamespaceNode currentNode);
        void MemoizeNamespaceTree(CodeGenNamespaceNode currentNode, Dictionary<string, CodeGenNamespaceNode> namespacesMemo);
        void MemoizeTypeNamespaces(ClassNode currentNode, Dictionary<string, string> memo);
        void MemoizeAssemblyTypeNamespaces(Assembly assembly, Dictionary<string, string> memo);
    }
}
