using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class NameSpaceService : INameSpaceService {
        private readonly ILogger _logger;
        private readonly INamePoolService _namePoolService;

        public NameSpaceService(ILogger logger, INamePoolService namePoolService) {
            _logger = logger;
            _namePoolService = namePoolService;
        }

        public void GetUniqueNamespaces(ClassNode currentNode, HashSet<string> namespaces) {
            namespaces.Add(currentNode.packageName);

            foreach (var child in currentNode.children) {
                GetUniqueNamespaces(child, namespaces);
            }
        }

        public CodeGenNamespaceNode BuildNamespaceTree(string[] namespaceNames, string previousNamespace, CodeGenNamespaceNode currentNode) {
            var rootLevel = namespaceNames
                .Where(i => i.Contains('/'))
                .Select(i => i.Split('/')[1])
                .Distinct()
                .ToArray();

            currentNode.namespaces = new CodeGenNamespaceNode[rootLevel.Length];
            for (var x = 0; x < rootLevel.Length; x++) {
                var fullBranchNamespace = $"{previousNamespace}/{rootLevel[x]}";
                var branchNamespace = rootLevel[x];

                var fullNamespace = fullBranchNamespace
                    .TrimStart('/')
                    .Replace('.', '_')
                    .Replace('/', '.');
                currentNode.namespaces[x] = new CodeGenNamespaceNode {
                    packageName = fullBranchNamespace,
                    fullNamespace = $"{CodeGenConstants.CODE_NAMESPACE}.{fullNamespace}",
                };

                var nameSpacePrefix = $"/{branchNamespace}/";
                var childNamespaceNames = namespaceNames
                    .Where(i => i.StartsWith(nameSpacePrefix))
                    .Select(i => i.Substring(nameSpacePrefix.Length - 1))
                    .ToArray();

                if (childNamespaceNames.Length == 0) {
                    continue;
                }

                currentNode.namespaces[x] = BuildNamespaceTree(childNamespaceNames, fullBranchNamespace, currentNode.namespaces[x]);
            }

            return currentNode;
        }

        public void MemoizeNamespaceTree(CodeGenNamespaceNode currentNode, Dictionary<string, CodeGenNamespaceNode> namespacesMemo) {
            namespacesMemo[currentNode.packageName] = currentNode;

            if (currentNode.namespaces is not null) {
                foreach (var namespaceNode in currentNode.namespaces) {
                    MemoizeNamespaceTree(namespaceNode, namespacesMemo);
                }
            }
        }

        public void MemoizeTypeNamespaces(ClassNode currentNode, Dictionary<string, string> memo) {
            var className = _namePoolService.GetSanitizedNameString(currentNode.ClassName);
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(memo, className, out var previouslyExisted);

            if (!previouslyExisted) {
                value = new StringBuilder(currentNode.packageName)
                    .Replace('/', '.')
                    .Insert(0, CodeGenConstants.CODE_NAMESPACE)
                    .ToString();
            }

            foreach (var child in currentNode.children) {
                MemoizeTypeNamespaces(child, memo);
            }
        }

        public void MemoizeAssemblyTypeNamespaces(Assembly assembly, Dictionary<string, string> memo) {
            foreach (var type in assembly.GetTypes()) {
                if (type.ContainsGenericParameters && type.Name.IndexOf('`') is > 0 and var index) {
                    var typeName = type.Name[..index];
                    memo[typeName] = type.Namespace!;
                }
                else {
                    memo[type.Name] = type.Namespace!;
                }
            }
        }
    }
}