using System.Runtime.CompilerServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class ImportResolver : IImportResolver {
        private readonly ILogger _logger;

        public ImportResolver(ILogger logger) {
            _logger = logger;
        }

        public void ApplyImports(CodeGenNamespaceNode currentNode, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            if (currentNode.classes is not null) {
                var imports = new HashSet<string>();

                foreach (var classNode in currentNode.classes) {
                    ResolveImportsForClass(classNode, currentNode.fullNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }

                if (imports.Count > 0) {
                    var namespaceImports = new string[imports.Count];
                    var i = 0;
                    foreach (var package in imports.Order()) {
                        namespaceImports[i] = package;
                        i++;
                    }

                    currentNode.imports = namespaceImports;
                }
            }

            if (currentNode.namespaces is not null) {
                foreach (var child in currentNode.namespaces) {
                    ApplyImports(child, customClassNamespaces, dotnetClassNamespaces);
                }
            }
        }

        private static void ResolveImportsForClass(CodeGenClassNode classNode, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            if (classNode.attributes is not null) {
                foreach (var attributeNode in classNode.attributes) {
                    TryAddAttributeAsImport(attributeNode.name, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (classNode.baseType is not null) {
                TryAddClassAsImport(classNode.baseType, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
            }

            if (classNode.interfaces is not null) {
                foreach (var interfaceNode in classNode.interfaces) {
                    ResolveImportsForInterface(interfaceNode, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (classNode.constructorNodes is not null) {
                foreach (var constructorNode in classNode.constructorNodes) {
                    ResolveImportsForConstructor(constructorNode, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (classNode.propertyNodes is not null) {
                foreach (var propertyNode in classNode.propertyNodes) {
                    ResolveImportsForProperty(propertyNode, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (classNode.methodNodes is not null) {
                foreach (var methodNode in classNode.methodNodes) {
                    ResolveImportsForMethod(methodNode, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (classNode.operatorNodes is not null) {
                foreach (var operatorNode in classNode.operatorNodes) {
                    ResolveImportsForOperator(operatorNode, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }
        }

        private static void ResolveImportsForInterface(CodeGenInterfaceNode interfaceNode, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            TryAddClassAsImport(interfaceNode.name, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);

            foreach (var methodNode in interfaceNode.methodNodes) {
                ResolveImportsForMethod(methodNode, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
            }
        }

        private static void ResolveImportsForConstructor(CodeGenConstructorNode constructorNode, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            if (constructorNode.attributes is not null) {
                foreach (var attributeNode in constructorNode.attributes) {
                    TryAddAttributeAsImport(attributeNode.name, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (constructorNode.arguments is not null) {
                foreach (var argumentNode in constructorNode.arguments) {
                    ResolveImportsForArgument(argumentNode, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }
        }

        private static void ResolveImportsForProperty(CodeGenPropertyNode propertyNode, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            // Properties use Unsafe.Write on the setter
            TryAddClassAsImport(nameof(Unsafe), currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);

            TryAddClassAsImport(propertyNode.returnType, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);

            if (propertyNode.attributes is not null) {
                foreach (var attributeNode in propertyNode.attributes) {
                    TryAddAttributeAsImport(attributeNode.name, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }
        }

        private static void ResolveImportsForMethod(CodeGenMethodNode methodNode, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            var returnType = methodNode.returnType;
            if (returnType is not VOID) {
                TryAddClassAsImport(returnType, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
            }

            if (methodNode.attributes is not null) {
                foreach (var attributeNode in methodNode.attributes) {
                    TryAddAttributeAsImport(attributeNode.name, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (methodNode.arguments is not null) {
                foreach (var argumentNode in methodNode.arguments) {
                    ResolveImportsForArgument(argumentNode, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (methodNode.bodyTypes is not null) {
                foreach (var type in methodNode.bodyTypes) {
                    TryAddClassAsImport(type, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }
        }

        private static void ResolveImportsForOperator(CodeGenOperatorNode operatorNode, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            TryAddClassAsImport(operatorNode.returnType, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
        }

        private static void ResolveImportsForArgument(CodeGenArgumentNode argumentNode, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            TryAddClassAsImport(argumentNode.type, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);

            if (argumentNode.attributes is not null) {
                foreach (var attributeNode in argumentNode.attributes) {
                    TryAddAttributeAsImport(attributeNode.name, currentNamespace, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }
        }

        private static void TryAddClassAsImport(string className, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            className = className.TrimEnd('*');

            if (customClassNamespaces.TryGetValue(className, out var customClassNamespace)) {
                if (!currentNamespace.StartsWith(customClassNamespace)) {
                    imports.Add(customClassNamespace);
                }

                return;
            }

            if (dotnetClassNamespaces.TryGetValue(className, out var dotnetClassNamespace)) {
                imports.Add(dotnetClassNamespace);
            }
        }

        private static void TryAddAttributeAsImport(string attributeName, string currentNamespace, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            var attributeType = $"{attributeName}Attribute";

            if (customClassNamespaces.TryGetValue(attributeType, out var customClassNamespace)) {
                if (!currentNamespace.StartsWith(customClassNamespace)) {
                    imports.Add(customClassNamespace);
                }

                return;
            }

            if (dotnetClassNamespaces.TryGetValue(attributeType, out var dotnetClassNamespace)) {
                imports.Add(dotnetClassNamespace);
            }
        }
    }
}