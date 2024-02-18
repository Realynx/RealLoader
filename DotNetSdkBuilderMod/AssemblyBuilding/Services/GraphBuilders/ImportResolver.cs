using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

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
                    ResolveImportsForClass(classNode, currentNode.packageName, imports, customClassNamespaces, dotnetClassNamespaces);
                }

                if (imports.Count > 0) {
                    var namespaceImports = new string[imports.Count];
                    var i = 0;
                    foreach (var package in imports) {
                        var import = new StringBuilder(package)
                            .TrimStart('/')
                            .Replace('/', '.')
                            .ToString();

                        namespaceImports[i] = $"{USING}{WHITE_SPACE}{import}{SEMICOLON}";
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

        private static void ResolveImportsForClass(CodeGenClassNode classNode, string currentPackage, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            if (classNode.attributes is not null) {
                foreach (var attributeNode in classNode.attributes) {
                    TryAddAttributeAsImport(attributeNode.name, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }

            if (classNode.baseType is not null) {
                TryAddClassAsImport(classNode.baseType, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);
            }

            if (classNode.constructorNodes is not null) {
                foreach (var constructorNode in classNode.constructorNodes) {
                    if (constructorNode.arguments is not null) {
                        foreach (var argumentNode in constructorNode.arguments) {
                            TryAddClassAsImport(argumentNode.type, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);
                        }
                    }
                }
            }

            if (classNode.propertyNodes is not null) {
                foreach (var propertyNode in classNode.propertyNodes) {
                    TryAddClassAsImport(propertyNode.returnType, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);

                    if (propertyNode.attributes is not null) {
                        foreach (var attributeNode in propertyNode.attributes) {
                            TryAddAttributeAsImport(attributeNode.name, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);
                        }
                    }
                }
            }

            if (classNode.methodNodes is not null) {
                foreach (var methodNode in classNode.methodNodes) {
                    var returnType = methodNode.returnType;
                    if (returnType != VOID) {
                        TryAddClassAsImport(returnType, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);
                    }

                    if (methodNode.attributes is not null) {
                        foreach (var attributeNode in methodNode.attributes) {
                            TryAddAttributeAsImport(attributeNode.name, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);
                        }
                    }

                    if (methodNode.arguments is not null) {
                        foreach (var argumentNode in methodNode.arguments) {
                            TryAddClassAsImport(argumentNode.type, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);
                        }
                    }
                }
            }

            if (classNode.operatorNodes is not null) {
                foreach (var operatorNode in classNode.operatorNodes) {
                    TryAddClassAsImport(operatorNode.returnType, currentPackage, imports, customClassNamespaces, dotnetClassNamespaces);
                }
            }
        }

        private static void TryAddClassAsImport(string className, string currentPackage, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            if (className == VOID) {
                return;
            }

            if (customClassNamespaces.TryGetValue(className, out var customClassNamespace) && !currentPackage.StartsWith(customClassNamespace)) {
                imports.Add(customClassNamespace);
            }

            if (dotnetClassNamespaces.TryGetValue(className, out var dotnetClassNamespace)) {
                imports.Add(dotnetClassNamespace);
            }
        }

        private static void TryAddAttributeAsImport(string attributeName, string currentPackage, HashSet<string> imports, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            var attributeType = $"{attributeName}Attribute";

            if (customClassNamespaces.TryGetValue(attributeType, out var customClassNamespace) && !currentPackage.StartsWith(customClassNamespace)) {
                imports.Add(customClassNamespace);
            }

            if (dotnetClassNamespaces.TryGetValue(attributeType, out var dotnetClassNamespace)) {
                imports.Add(dotnetClassNamespace);
            }
        }
    }
}