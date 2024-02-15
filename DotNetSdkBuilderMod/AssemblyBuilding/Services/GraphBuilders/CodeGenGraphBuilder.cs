using System.Diagnostics;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class CodeGenGraphBuilder : ICodeGenGraphBuilder {
        private readonly ILogger _logger;
        private readonly INameDistanceService _nameDistanceService;

        public CodeGenGraphBuilder(ILogger logger, INameDistanceService nameDistanceService) {
            _logger = logger;
            _nameDistanceService = nameDistanceService;
        }

        // TODO: Fine tune this.
        // Do not make it user-configurable
        const int GROUPING_DISTANCE = 5;

        public CodeGenAssemblyNode[] BuildAssemblyGraphs(ClassNode rootNode) {
            _logger.Debug("Getting distinct namespaces...");
            var distinctNamespaces = TimedDistinctNamespaces(rootNode);

            DebugUtilities.WaitForDebuggerAttach();

            _logger.Debug("Building namespace tree...");
            var namespaceTree = TimedNamespaceTree(rootNode, distinctNamespaces);

            foreach (var rootNamespace in namespaceTree) {
                _logger.Debug($"Root Name: {rootNamespace.nameSpace}");

                PrintTree(rootNamespace);
            }

            void PrintTree(CodeGenNamespaceNode current, int tabIndex = 0) {
                _logger.Debug($"{new string(' ', tabIndex * 4)}- {current.nameSpace}");
                if (current.namespaces != null) {
                    foreach (var child in current.namespaces) {
                        PrintTree(child, tabIndex + 1);
                    }
                }
            }

            return [];
        }

        private string[] TimedDistinctNamespaces(ClassNode rootNode) {
            var timer = new Stopwatch();
            timer.Start();
            var distinctNamespaces = GetAllNamespaces(rootNode).Distinct();
            timer.Stop();

            _logger.Debug($"Distinct namespace; {timer.ElapsedMilliseconds} ms to build.");
            return distinctNamespaces.ToArray();
        }

        private string[] GetAllNamespaces(ClassNode currentNode) {
            var childrenNamespaces = new List<string> {
                currentNode.packageName
            };

            foreach (var child in currentNode.children) {
                var childNamespaces = GetAllNamespaces(child);
                childrenNamespaces.AddRange(childNamespaces);
            }

            return childrenNamespaces.ToArray();
        }

        private CodeGenNamespaceNode[] TimedNamespaceTree(ClassNode rootNode, string[] namespaces) {
            var timer = new Stopwatch();
            timer.Start();
            var namespaceTree = new CodeGenNamespaceNode();
            _ = BuildAllNamespaces(namespaces, namespaceTree);
            timer.Stop();

            _logger.Debug($"Built namespace tree; {timer.ElapsedMilliseconds} ms to build.");
            return namespaceTree.namespaces;
        }

        private CodeGenNamespaceNode BuildAllNamespaces(string[] namespaceNames, CodeGenNamespaceNode currentNode) {
            var rootLevel = namespaceNames
                .Where(i => i.Contains('/'))
                .Select(i => i.Split('/')[1])
                .Distinct()
                .ToArray();

            currentNode.namespaces = new CodeGenNamespaceNode[rootLevel.Length];
            for (var x = 0; x < rootLevel.Length; x++) {
                var branchNamespace = rootLevel[x];

                currentNode.namespaces[x] = new CodeGenNamespaceNode() {
                    nameSpace = branchNamespace
                };

                var nameSpacePrefix = $"/{currentNode.namespaces[x].nameSpace}/";
                var childNamespaceNames = namespaceNames
                    .Where(i => i.StartsWith(nameSpacePrefix))
                    .Select(i => i.Substring(nameSpacePrefix.Length - 1))
                    .ToArray();

                if (childNamespaceNames.Length == 0 || childNamespaceNames is [""]) {
                    continue;
                }

                currentNode.namespaces[x] = BuildAllNamespaces(childNamespaceNames, currentNode.namespaces[x]);
            }

            return currentNode;
        }
    }
}
