using System.Runtime.InteropServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class ReflectedGraphBuilder : IReflectedGraphBuilder {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;
        private readonly UnrealReflection _unrealReflection;

        private List<UObjectBase> _everyLoadedObjects;
        private Dictionary<string, List<UClass>> _classMemo;

        public ReflectedGraphBuilder(ILogger logger, IGlobalObjects globalObjects, UnrealReflection unrealReflection) {
            _logger = logger;
            _globalObjects = globalObjects;
            _unrealReflection = unrealReflection;
        }

        public ClassNode? BuildRootNode() {
            _logger.Debug("Building root node graph");
            _everyLoadedObjects = _globalObjects.EnumerateEverything().ToList();
            _classMemo = new();

            var rootNode = MemoizeClassNodes();
            if (rootNode is null) {
                return rootNode;
            }

            rootNode.children = FindChildren(rootNode);
            return rootNode;
        }

        private unsafe ClassNode? MemoizeClassNodes() {
            ClassNode? rootNode = null;

            foreach (var obj in _everyLoadedObjects) {
                var objectClass = *obj.classPrivate;
                var className = _globalObjects.GetNameString(objectClass.ObjectName);

                if (objectClass.baseUStruct.superStruct is null) {
                    var functions = _unrealReflection.GetTypeFunctions(objectClass);
                    var properties = _unrealReflection.GetTypeFields(objectClass);
                    rootNode = new ClassNode() {
                        functions = functions.ToArray(),
                        properties = properties.ToArray(),
                        nodeClass = objectClass
                    };
                }
                else {
                    var super = *objectClass.baseUStruct.superStruct;
                    var superName = _globalObjects.GetNameString(super.ObjectName);

                    ref var collectionValue = ref CollectionsMarshal
                        .GetValueRefOrAddDefault(_classMemo, superName, out var previouslyExisted);

                    if (!previouslyExisted) {
                        collectionValue = new List<UClass>();
                    }

                    collectionValue!.Add(objectClass);
                }
            }

            return rootNode;
        }

        private unsafe ClassNode[] FindChildren(ClassNode currentnode) {
            var children = new List<ClassNode>();
            var currentNodeName = _globalObjects.GetNameString(currentnode.ClassName);


            var memoChildren = _classMemo[currentNodeName].ToArray();
            if (memoChildren is null) {
                return children.ToArray();
            }

            foreach (var memoChild in memoChildren) {
                children.Add(new ClassNode() {
                    functions = _unrealReflection.GetTypeFunctions(memoChild).ToArray(),
                    properties = _unrealReflection.GetTypeFields(memoChild).ToArray(),
                    nodeClass = memoChild,
                    parent = currentnode
                });
            }

            foreach (var child in children) {
                child.children = FindChildren(currentnode);
            }

            return children.ToArray();
        }
    }
}
