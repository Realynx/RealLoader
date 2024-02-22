using System.Buffers;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class UObjectInteropExtensionsBuilder : IUObjectInteropExtensionsBuilder {
        private readonly ILogger _logger;

        public UObjectInteropExtensionsBuilder(ILogger logger) {
            _logger = logger;
        }

        public void PopulateNamespaceNode(CodeGenNamespaceNode extensionsNamespaceNode, IReadOnlyList<int> functionArgCounts) {
            var methods = new CodeGenMethodNode[functionArgCounts.Count];
            for (var i = 0; i < functionArgCounts.Count; i++) {
                methods[i] = GetExtensionMethod(functionArgCounts[i]);
            }

            var modifiers = $"{PUBLIC}{WHITE_SPACE}{STATIC}";

            var className = U_OBJECT_INTEROP_EXTENSIONS_CLASS_NAME;

            extensionsNamespaceNode.classes = new[] {
                new CodeGenClassNode {
                    methodNodes = methods,
                    modifier = modifiers,
                    name = className,
                }
            };

            extensionsNamespaceNode.imports = new [] {
                $"{nameof(System)}{DOT}{nameof(System.Buffers)}"
            };
        }

        private CodeGenMethodNode GetExtensionMethod(int functionArgCount) {
            if (functionArgCount == 0) {
                return GetDefaultExtensionMethod();
            }

            return GetGenericExtensionMethod(functionArgCount);
        }

        private CodeGenMethodNode GetDefaultExtensionMethod() {
            var modifiers = $"{PUBLIC}{WHITE_SPACE}{STATIC}{WHITE_SPACE}{UNSAFE}";

            var methodName = CODE_GEN_INTEROP_INVOKE_METHOD_NAME;

            var returnType = VOID;

            var methodArgs = new[] {
                GetExtensionArgument(),
                GetFunctionAddressArgument(),
            };

            var body = new List<string> {
                "const int ARGS_SIZE = 0x1000; // Magic number for some reason",
                string.Empty
            };
            body.AddRange(GetArgMemoryAllocationSegment());
            body.Add(string.Empty);
            body.AddRange(GetProcessEventCallSegment());

            var bodyTypes = new[] {
                nameof(UIntPtr),
                $"{nameof(MemoryPool<nint>)}",
            };

            return new CodeGenMethodNode {
                modifier = modifiers,
                name = methodName,
                returnType = returnType,
                arguments = methodArgs,
                body = body.ToArray(),
                bodyTypes = bodyTypes,
            };
        }

        private CodeGenMethodNode GetGenericExtensionMethod(int genericArgCount) {
            var modifiers = $"{PUBLIC}{WHITE_SPACE}{STATIC}{WHITE_SPACE}{UNSAFE}";

            var methodName = CODE_GEN_INTEROP_INVOKE_METHOD_NAME;

            var returnType = VOID;

            var methodArgs = new CodeGenArgumentNode[genericArgCount + 2];
            methodArgs[0] = GetExtensionArgument();
            methodArgs[1] = GetFunctionAddressArgument();
            for (var i = 0; i < genericArgCount; i++) {
                methodArgs[i + 2] = new CodeGenArgumentNode
                {
                    type = $"T{i + 1}",
                    name = $"arg{i + 1}"
                };
            }

            var genericTypes = new string[genericArgCount];
            for (var i = 0; i < genericArgCount; i++) {
                genericTypes[i] = $"T{i + 1}";
            }

            var body = new List<string> {
                $"const int ARGS_SIZE = {genericArgCount};",
                string.Empty
            };
            body.AddRange(GetArgMemoryAllocationSegment());
            body.Add(string.Empty);
            for (var i = 0; i < genericArgCount; i++) {
                body.Add($"span[{i}] = objectInterop.{nameof(UObjectInterop.GetObjectAddress)}(ref arg{i + 1});");
            }
            body.Add(string.Empty);
            body.AddRange(GetProcessEventCallSegment());

            var bodyTypes = new[] {
                nameof(UIntPtr),
                $"{nameof(MemoryPool<nint>)}",
            };

            return new CodeGenMethodNode {
                modifier = modifiers,
                name = methodName,
                returnType = returnType,
                arguments = methodArgs,
                genericTypes = genericTypes,
                body = body.ToArray(),
                bodyTypes = bodyTypes,
            };
        }

        private static CodeGenArgumentNode GetExtensionArgument() {
            return new CodeGenArgumentNode
            {
                modifier = THIS,
                type = nameof(UObjectInterop),
                name = "objectInterop",
            };
        }

        private static CodeGenArgumentNode GetFunctionAddressArgument() {
            return new CodeGenArgumentNode
            {
                type = INT,
                name = "functionStruct",
            };
        }

        private static IEnumerable<string> GetArgMemoryAllocationSegment() {
            yield return "using var arguments = MemoryPool<nint>.Shared.Rent(ARGS_SIZE);";
            yield return "var span = arguments.Memory.Span;";
            yield return "span[ARGS_SIZE..].Fill(0);";
        }

        private static IEnumerable<string> GetProcessEventCallSegment() {
            yield return "fixed (void* argsPtr = &span.GetPinnableReference()) {";
            yield return $"    objectInterop.{nameof(UObjectInterop.Invoke)}(functionStruct, argsPtr);";
            yield return "}";
        }

        public CodeGenNamespaceNode GetScaffoldNamespaceNode() {
            var namespaceParts = U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE.Split('.');

            return new CodeGenNamespaceNode {
                packageName = namespaceParts[0],
                name = namespaceParts[0],
                namespaces = new [] {
                    new CodeGenNamespaceNode
                    {
                        packageName = U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE,
                        name = namespaceParts[1]
                    }
                }
            };
        }
    }
}