using System.Buffers;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Models;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class UObjectInteropExtensionsBuilder : IUObjectInteropExtensionsBuilder {
        private readonly ILogger _logger;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;

        public UObjectInteropExtensionsBuilder(ILogger logger, ICodeGenAttributeNodeFactory attributeNodeFactory) {
            _logger = logger;
            _attributeNodeFactory = attributeNodeFactory;
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

            extensionsNamespaceNode.imports = new[] {
                $"{nameof(System)}{DOT}{nameof(System.Buffers)}",
                $"{nameof(System)}{DOT}{nameof(System.Runtime)}",
                $"{nameof(System)}{DOT}{nameof(System.Runtime)}{DOT}{nameof(System.Runtime.CompilerServices)}",
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
                "const int ARGS_SIZE = byte.MaxValue; // The params counter is a byte",
                string.Empty
            };
            body.AddRange(GetArgMemoryAllocationSegment(true));
            body.Add(string.Empty);
            body.AddRange(GetProcessEventCallSegment());

            var bodyTypes = new[] {
                nameof(UIntPtr),
                nameof(MemoryPool<nint>),
            };

            var attributes = new[] {
                _attributeNodeFactory.GenerateAttribute(COMPILER_GENERATED_ATTRIBUTE)
            };

            return new CodeGenMethodNode {
                modifier = modifiers,
                name = methodName,
                returnType = returnType,
                arguments = methodArgs,
                body = body.ToArray(),
                bodyTypes = bodyTypes,
                attributes = attributes,
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
            body.AddRange(GetArgMemoryAllocationSegment(false));
            body.Add(string.Empty);
            for (var i = 0; i < genericArgCount; i++) {
                body.Add($"span[{i}] = {nameof(UObjectInterop)}.{nameof(UObjectInterop.GetObjectAddress)}(ref arg{i + 1});");
            }
            body.Add(string.Empty);
            body.AddRange(GetProcessEventCallSegment());

            var bodyTypes = new[] {
                nameof(UIntPtr),
                nameof(MemoryPool<nint>),
            };

            var attributes = new[] {
                _attributeNodeFactory.GenerateAttribute(COMPILER_GENERATED_ATTRIBUTE)
            };

            return new CodeGenMethodNode {
                modifier = modifiers,
                name = methodName,
                returnType = returnType,
                arguments = methodArgs,
                genericTypes = genericTypes,
                body = body.ToArray(),
                bodyTypes = bodyTypes,
                attributes = attributes,
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
                type = $"{nameof(UFunction)}{STAR}",
                name = "functionStruct",
            };
        }

        private static IEnumerable<string> GetArgMemoryAllocationSegment(bool fillAll) {
            yield return "using var arguments = MemoryPool<nint>.Shared.Rent(ARGS_SIZE);";
            yield return "var span = arguments.Memory.Span;";
            if (fillAll) {
                yield return "span.Clear();";
            }
            else {
                yield return "span[ARGS_SIZE..].Clear();";
            }
        }

        private static IEnumerable<string> GetProcessEventCallSegment() {
            yield return "fixed (void* argsPtr = &span.GetPinnableReference()) {";
            yield return $"    objectInterop.{nameof(UObjectInterop.Invoke)}(functionStruct, argsPtr);";
            yield return "}";
        }

        public CodeGenNamespaceNode GetScaffoldNamespaceNode(string baseNamespace) {
            var namespaceParts = U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE.Split('.');
            var fullUpperNamespace = $"{baseNamespace}.{namespaceParts[0]}";
            var fullLowerNamespace = $"{baseNamespace}.{U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE}";

            return new CodeGenNamespaceNode {
                packageName = fullUpperNamespace,
                fullNamespace = fullUpperNamespace,
                namespaces = new[] {
                    new CodeGenNamespaceNode
                    {
                        packageName = fullLowerNamespace,
                        fullNamespace = fullLowerNamespace,
                    }
                }
            };
        }
    }
}