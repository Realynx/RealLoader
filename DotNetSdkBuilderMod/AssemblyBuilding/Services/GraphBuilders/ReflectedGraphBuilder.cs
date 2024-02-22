using System.Runtime.InteropServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

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

        public unsafe ClassNode? BuildRootNode() {
            _logger.Debug("Building root node graph");

            _everyLoadedObjects = _globalObjects.EnumerateEverything();
            _classMemo = new Dictionary<string, HashSet<nint>>();

            var rootNode = MemoizeClassNodes();
            if (rootNode is null) {
                return rootNode;
            }

            rootNode.children = FindChildren(rootNode);
            return rootNode;
        }

        private unsafe ClassNode? MemoizeClassNodes() {
            ClassNode? rootNode = null;

            foreach (UObjectBase* pObject in _everyLoadedObjects) {
                var pObjectClass = pObject->classPrivate;

                if (pObjectClass->baseUStruct.superStruct is null) {
                    var functions = _unrealReflection.GetTypeFunctions(pObjectClass);
                    var properties = _unrealReflection.GetTypeFields(pObjectClass);
                    var packageName = _packageNameGenerator.GetPackageName((UObjectBaseUtility*)pObject);

                    rootNode = new ClassNode() {
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

        private unsafe ClassNode[] FindChildren(ClassNode currentnode) {
            var children = new List<ClassNode>();
            var currentNodeName = _namePoolService.GetNameString(currentnode.ClassName);

            if (!_classMemo.TryGetValue(currentNodeName, out var memoChildren)) {
                return Array.Empty<ClassNode>();
            }

            _classMemo.Remove(currentNodeName);

            if (memoChildren.Count < 1) {
                return Array.Empty<ClassNode>();
            }

            foreach (var memoChild in memoChildren) {
                var classBase = currentnode.nodeClass->baseUStruct.baseUfield.baseUObject;
                var packageName = string.Intern(_packageNameGenerator.GetPackageName((UObjectBaseUtility*)&classBase));

                children.Add(new ClassNode() {
                    functions = _unrealReflection.GetTypeFunctions((UClass*)memoChild),
                    properties = _unrealReflection.GetTypeFields((UClass*)memoChild),
                    packageName = packageName,
                    nodeClass = (UClass*)memoChild,
                    parent = currentnode
                });
            }

            foreach (var child in children) {
                child.children = FindChildren(child);
            }

            return children.ToArray();
        }
    }
}
