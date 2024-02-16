using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class ImportResolver : IImportResolver {
        public void ApplyImports(CodeGenNamespaceNode current, Dictionary<string, string> classNamespaces) {
            var imports = new HashSet<string>();

            // Lord have mercy on my soul for the amount of indentation
            if (current.classes is not null) {
                foreach (var classNode in current.classes) {
                    if (classNode.baseType != null) {
                        TryAddClassAsImport(classNode.baseType);
                    }

                    if (classNode.propertyNodes is not null) {
                        foreach (var propertyNode in classNode.propertyNodes) {
                            TryAddClassAsImport(propertyNode.returnType);
                        }
                    }

                    if (classNode.methodNodes is not null) {
                        foreach (var methodNode in classNode.methodNodes) {
                            var returnType = methodNode.returnType;
                            if (returnType != CodeGenConstants.VOID) {
                                TryAddClassAsImport(returnType);
                            }

                            if (methodNode.arguments is not null) {
                                foreach (var arg in methodNode.arguments) {
                                    TryAddClassAsImport(arg.type);
                                }
                            }
                        }
                    }
                }
            }

            var importsBuilder = new StringBuilder();
            foreach (var import in imports) {
                importsBuilder.Append($"{CodeGenConstants.USING} ");
                importsBuilder.Append(import.AsSpan(1));
                importsBuilder.AppendLine(";");
            }

            current.imports = importsBuilder
                .Replace('/', '.')
                .ToString();

            if (current.namespaces is not null) {
                foreach (var child in current.namespaces) {
                    ApplyImports(child, classNamespaces);
                }
            }

            void TryAddClassAsImport(string className) {
                var currentNameSpace = current.fullNameSpace;

                if (classNamespaces.TryGetValue(className, out var classNamespace) && !currentNameSpace.StartsWith(classNamespace)) {
                    imports.Add(classNamespace);
                }
            }
        }
    }
}