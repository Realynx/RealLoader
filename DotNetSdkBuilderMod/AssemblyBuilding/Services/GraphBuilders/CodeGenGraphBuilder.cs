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
                _logger.Debug($"Root Name: {rootNamespace}");

                PrintTree(rootNamespace);
            }

            void PrintTree(CodeGenNamespaceNode current, int tabIndex = 0) {
                _logger.Debug($"{new string(' ', tabIndex * 4)}- {current.nameSpace}");
                foreach (var child in current.namespaces) {
                    PrintTree(child, tabIndex++);
                }
            }


            return [];
        }

        //private class CodeGenNamespaceNode2 {
        //    public List<CodeGenNamespaceNode> namespaces;
        //    public List<CodeGenClassNode> classes;

        //    public string nameSpace;
        //    public string imports;
        //}

        //private void GetNameSpaceNodes(ClassNode currentNode, List<CodeGenNamespaceNode2> rootNamespaceNodes) {
        //    var currentNameSpace = currentNode.packageName;

        //    var selectedRootNode = rootNamespaceNodes.FirstOrDefault(x => currentNameSpace.AsSpan(1).StartsWith(x.nameSpace));
        //    if (selectedRootNode == null) {
        //        selectedRootNode = new CodeGenNamespaceNode2() {
        //            nameSpace = currentNameSpace.Split('/')[0]
        //        };
        //    }

        //    rootNamespaceNodes.Add(selectedRootNode);
        //    AddNameSpace(currentNode, selectedRootNode);

        //    var childNameSpaces = new List<CodeGenNamespaceNode>();
        //    foreach (var child in currentNode.children) {
        //        childNameSpaces.Add(GetNameSpaces(child, depth + 1));
        //    }

        //    return
        //}

        //private void AddNameSpace(ClassNode node, CodeGenNamespaceNode2 namespaceNode) {
        //    if (node.packageName.AsSpan().Trim('/').SequenceEqual(namespaceNode.nameSpace)) {

        //    }
        //}

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

        private CodeGenNamespaceNode BuildAllNamespaces(IEnumerable<string> namespaceNames, CodeGenNamespaceNode currentNode) {
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

                var nameSpacePrefix = $"/{currentNode.namespaces[x].nameSpace}";
                var childNamespaceNames = namespaceNames
                    .Where(i => i.StartsWith(nameSpacePrefix))
                    .Select(i => i.Substring(nameSpacePrefix.Length));

                currentNode.namespaces[x] = BuildAllNamespaces(childNamespaceNames, currentNode);
            }

            return currentNode;
        }
    }
}
