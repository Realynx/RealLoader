using System.Diagnostics;
using System.Runtime.InteropServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public unsafe class ReflectedGraphBuilder : IReflectedGraphBuilder {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;
        private readonly IUnrealReflection _unrealReflection;
        private readonly IPackageNameGenerator _packageNameGenerator;
        private readonly INamePoolService _namePoolService;

        private UObjectBase*[] _everyLoadedObjects;
        private Dictionary<string, HashSet<nint>> _classMemo;

        public ReflectedGraphBuilder(ILogger logger, IGlobalObjects globalObjects, IUnrealReflection unrealReflection,
            IPackageNameGenerator packageNameGenerator, INamePoolService namePoolService) {
            _logger = logger;
            _globalObjects = globalObjects;
            _unrealReflection = unrealReflection;
            _packageNameGenerator = packageNameGenerator;
            _namePoolService = namePoolService;
        }

        public ClassNode? BuildRootNode() {
            _logger.Debug("Building root node graph");

            _everyLoadedObjects = _globalObjects.EnumerateEverything();
            _classMemo = new Dictionary<string, HashSet<nint>>();

            var rootNode = MemoizeClassNodes();
            if (rootNode is null) {
                return rootNode;
            }

            rootNode.children = FindChildren(rootNode);

            LinkInheritedMembers(rootNode);

            return rootNode;
        }

        private ClassNode? MemoizeClassNodes() {
            ClassNode? rootNode = null;

            foreach (UObjectBase* pObject in _everyLoadedObjects) {
                var pObjectClass = pObject->classPrivate;

                if (pObjectClass->baseUStruct.superStruct is null) {
                    GetClassMemberNodes(pObjectClass, out var properties, out var functions);
                    var packageName = _packageNameGenerator.GetPackageName((UObjectBaseUtility*)pObject);

                    rootNode = new ClassNode {
                        functions = functions,
                        properties = properties,
                        packageName = packageName,
                        nodeClass = pObjectClass
                    };
                    continue;
                }

                var super = pObjectClass->baseUStruct.superStruct;
                var superName = _namePoolService.GetNameString(super->ObjectName);

                ref var collectionValue = ref CollectionsMarshal
                    .GetValueRefOrAddDefault(_classMemo, superName, out var previouslyExisted);

                if (!previouslyExisted) {
                    collectionValue = new HashSet<nint>();
                }

                collectionValue!.Add((nint)pObjectClass);
            }

            return rootNode;
        }

        private ClassNode[] FindChildren(ClassNode currentNode) {
            var children = new List<ClassNode>();
            var currentNodeName = _namePoolService.GetNameString(currentNode.ClassName);

            if (!_classMemo.Remove(currentNodeName, out var memoChildren)) {
                return Array.Empty<ClassNode>();
            }

            if (memoChildren.Count < 1) {
                return Array.Empty<ClassNode>();
            }

            foreach (var memoChild in memoChildren) {
                GetClassMemberNodes((UClass*)memoChild, out var properties, out var functions);
                var classBase = currentNode.nodeClass->baseUStruct.baseUfield.baseUObject;
                var packageName = string.Intern(_packageNameGenerator.GetPackageName((UObjectBaseUtility*)&classBase));

                children.Add(new ClassNode {
                    functions = functions,
                    properties = properties,
                    packageName = packageName,
                    nodeClass = (UClass*)memoChild,
                    parent = currentNode
                });
            }

            foreach (var child in children) {
                child.children = FindChildren(child);
            }

            return children.ToArray();
        }

        private void GetClassMemberNodes(UClass* uClass, out PropertyNode[] properties, out FunctionNode[] functions) {
            var classProperties = _unrealReflection.GetTypeFields(uClass);

            properties = new PropertyNode[classProperties.Length];
            for (var i = 0; i < properties.Length; i++) {
                properties[i] = new PropertyNode { nodeProperty = classProperties[i] };
            }

            var classFunctions = _unrealReflection.GetTypeFunctions(uClass);

            functions = new FunctionNode[classFunctions.Length];
            for (var i = 0; i < functions.Length; i++) {
                functions[i] = new FunctionNode { nodeFunction = classFunctions[i] };
            }
        }

        private void LinkInheritedMembers(ClassNode currentNode,
            IReadOnlyDictionary<string, PropertyNode>? inheritedProperties = null,
            IReadOnlyDictionary<string, FunctionNode>? inheritedFunctions = null) {
            // TODO: Support renaming duplicate member names (Foo(), Foo(), Foo() -> Foo(), Foo_1(), Foo_2())
            var properties = LinkInheritedProperties(currentNode, inheritedProperties);
            var functions = LinkInheritedFunctions(currentNode, inheritedFunctions);

            currentNode.properties = properties.Select(x => x.Value).ToArray();
            currentNode.functions = functions.Select(x => x.Value).ToArray();

            foreach (var child in currentNode.children) {
                LinkInheritedMembers(child, properties, functions);
            }
        }

        private Dictionary<string, PropertyNode> LinkInheritedProperties(ClassNode currentNode, IReadOnlyDictionary<string, PropertyNode>? inheritedProperties) {
            var properties = new Dictionary<string, PropertyNode>();
            foreach (var propertyNode in currentNode.properties) {
                var propertyName = _namePoolService.GetNameString(propertyNode.PropertyName);

                ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(properties, propertyName, out var previouslyExisted);

                if (previouslyExisted) {
                    continue;
                }

                value = propertyNode;
            }

            if (inheritedProperties != null) {
                foreach (var (signature, propertyNode) in inheritedProperties) {
                    ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(properties, signature, out var previouslyExisted);

                    if (previouslyExisted) {
                        continue;
                    }

                    value = new PropertyNode { nodeProperty = propertyNode.nodeProperty, inheritedFrom = propertyNode };
                }
            }

            return properties;
        }

        private Dictionary<string, FunctionNode> LinkInheritedFunctions(ClassNode currentNode, IReadOnlyDictionary<string, FunctionNode>? inheritedFunctions) {
            var functions = new Dictionary<string, FunctionNode>();
            foreach (var functionNode in currentNode.functions) {
                GetFunctionNodeSignature(functionNode, out var functionName, out var returnType, out var parameterTypes);

                var signature = $"{functionName}{returnType}{string.Concat(parameterTypes)}";
                ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(functions, signature, out var previouslyExisted);

                if (previouslyExisted) {
                    continue;
                }

                value = functionNode;
            }

            if (inheritedFunctions != null) {
                foreach (var (signature, functionNode) in inheritedFunctions) {
                    ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(functions, signature, out var previouslyExisted);

                    if (previouslyExisted) {
                        continue;
                    }

                    value = new FunctionNode { nodeFunction = functionNode.nodeFunction, inheritedFrom = functionNode };
                }
            }

            return functions;
        }

        private void GetFunctionNodeSignature(FunctionNode functionNode, out string functionName, out string returnType, out IEnumerable<string> parameterTypes) {
            functionName = _namePoolService.GetNameString(functionNode.FunctionName);
            var parameters = _unrealReflection.GetFunctionSignature(functionNode.nodeFunction, out var returnValue, out var returnValueIndex);

            returnType = returnValue is null ? string.Empty : _namePoolService.GetNameString(returnValue->ObjectName);

            var paramCount = Math.Max(0, parameters.Length - (returnValue is null ? 0 : 1));
            var paramsList = new List<string>(paramCount);
            for (var i = 0; i < parameters.Length; i++) {
                if (i == returnValueIndex.Value) {
                    continue;
                }

                var type = _namePoolService.GetNameString(parameters[i]->classPrivate->ObjectName);
                paramsList.Add(type);
            }

            Debug.Assert(paramsList.Count == paramCount);
            parameterTypes = paramsList;
        }
    }
}
