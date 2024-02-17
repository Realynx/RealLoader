using System.Runtime.InteropServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;

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

                currentNode.namespaces[x] = new CodeGenNamespaceNode {
                    packageName = fullBranchNamespace,
                    name = branchNamespace
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
            var className = _namePoolService.GetNameString(currentNode.ClassName);
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(memo, className, out var previouslyExisted);

            if (!previouslyExisted) {
                value = currentNode.packageName;
            }

            foreach (var child in currentNode.children) {
                MemoizeTypeNamespaces(child, memo);
            }
        }
    }
}