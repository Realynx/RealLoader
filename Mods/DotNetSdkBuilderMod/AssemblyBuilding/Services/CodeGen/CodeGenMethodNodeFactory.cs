using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Models;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenMethodNodeFactory : ICodeGenMethodNodeFactory {
        private readonly ILogger _logger;
        private readonly INamePoolService _namePoolService;
        private readonly IUnrealReflection _unrealReflection;
        private readonly INameCollisionService _nameCollisionService;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;

        public CodeGenMethodNodeFactory(ILogger logger, INamePoolService namePoolService, IUnrealReflection unrealReflection,
            INameCollisionService nameCollisionService, ICodeGenAttributeNodeFactory attributeNodeFactory) {
            _logger = logger;
            _namePoolService = namePoolService;
            _unrealReflection = unrealReflection;
            _nameCollisionService = nameCollisionService;
            _attributeNodeFactory = attributeNodeFactory;
        }

        public unsafe CodeGenMethodNode GenerateOwnedMethodNode(FunctionNode functionNode, Index functionIndex) {
            var modifiers = "public virtual";

            var nonSanitizedMethodName = _namePoolService.GetNameString(functionNode.FunctionName);
            var methodName = _namePoolService.GetSanitizedNameString(functionNode.FunctionName);
            methodName = _nameCollisionService.GetNonCollidingName(methodName);

            var attributes = new[] {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"\"{nonSanitizedMethodName}\"")
            };

            var parameters = _unrealReflection.GetFunctionSignature(functionNode.nodeFunction, out var returnValue, out var returnValueIndex);
            string returnType;
            if (returnValue is not null) {
                var signatureName = returnValue->classPrivate->ObjectName;
                var signatureString = _namePoolService.GetSanitizedNameString(signatureName);
                returnType = signatureString;
            }
            else {
                returnType = VOID;
            }

            CodeGenArgumentNode[]? methodArgs = null;
            if (parameters.Length > 0) {
                var argNames = new HashSet<string>();

                methodArgs = new CodeGenArgumentNode[parameters.Length];
                for (var i = 0; i < parameters.Length; i++) {
                    var currentParam = parameters[i];

                    var type = _namePoolService.GetSanitizedNameString(currentParam->classPrivate->ObjectName);
                    var name = _namePoolService.GetSanitizedNameString(currentParam->ObjectName);

                    methodArgs[i] = new CodeGenArgumentNode {
                        type = type,
                        name = _nameCollisionService.GetNonCollidingName(name, argNames)
                    };
                }
            }

            string[] body;
            if (methodArgs is not null) {
                if (returnValue is null) {
                    body = new[] {
                        $"this.{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}({nameof(UObjectInterop.GetFunctionStructPointer)}({functionIndex}), {string.Join(", ", methodArgs.Select(x => x.name))});",
                    };
                }
                else {
                    // TODO: Return values will cause NullReferenceException
                    var sb = new StringBuilder($"this.{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}({nameof(UObjectInterop.GetFunctionStructPointer)}({functionIndex})");
                    var argsBeforeReturnValue = methodArgs.Take(returnValueIndex.Value).Select(x => x.name).ToArray();
                    if (argsBeforeReturnValue.Length > 0) {
                        sb.Append(", ");
                        sb.Append(string.Join(", ", argsBeforeReturnValue));
                    }

                    sb.Append($", {CODE_GEN_INTEROP_RETURN_VALUE_NAME}");

                    var argsAfterReturnValue = methodArgs.Skip(returnValueIndex.Value).Select(x => x.name).ToArray();
                    if (argsAfterReturnValue.Length > 0) {
                        sb.Append(", ");
                        sb.Append(string.Join(", ", argsAfterReturnValue));
                    }

                    sb.Append(");");

                    body = new[] {
                        $"{returnType} {CODE_GEN_INTEROP_RETURN_VALUE_NAME} = default;",
                        sb.ToString(),
                        $"return {CODE_GEN_INTEROP_RETURN_VALUE_NAME};",
                    };
                }
            }
            else {
                if (returnValue is null) {
                    body = new[] {
                        $"this.{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}({nameof(UObjectInterop.GetFunctionStructPointer)}({functionIndex}));",
                    };
                }
                else {
                    // TODO: Return values will cause NullReferenceException
                    body = new[] {
                        $"{returnType} {CODE_GEN_INTEROP_RETURN_VALUE_NAME} = default;",
                        $"this.{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}({nameof(UObjectInterop.GetFunctionStructPointer)}({functionIndex}), {CODE_GEN_INTEROP_RETURN_VALUE_NAME});",
                        $"return {CODE_GEN_INTEROP_RETURN_VALUE_NAME};",
                    };
                }
            }

            var bodyTypes = new[] {
                U_OBJECT_INTEROP_EXTENSIONS_CLASS_NAME,
            };

            return new CodeGenMethodNode {
                modifier = modifiers,
                name = methodName,
                attributes = attributes,
                returnType = returnType,
                arguments = methodArgs,
                body = body,
                bodyTypes = bodyTypes,
            };
        }

        public unsafe CodeGenMethodNode GenerateInheritedMethodNode(FunctionNode functionNode) {
            var modifiers = "public override";

            var nonSanitizedMethodName = _namePoolService.GetNameString(functionNode.FunctionName);
            var methodName = _namePoolService.GetSanitizedNameString(functionNode.FunctionName);
            methodName = _nameCollisionService.GetNonCollidingName(methodName);

            var attributes = new[] {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"\"{nonSanitizedMethodName}\""),
                _attributeNodeFactory.GenerateAttribute(COMPILER_GENERATED_ATTRIBUTE)
            };

            var parameters = _unrealReflection.GetFunctionSignature(functionNode.nodeFunction, out var returnValue, out _);
            string returnType;
            if (returnValue is not null) {
                var signatureName = returnValue->classPrivate->ObjectName;
                var signatureString = _namePoolService.GetSanitizedNameString(signatureName);
                returnType = signatureString;
            }
            else {
                returnType = VOID;
            }

            CodeGenArgumentNode[]? methodArgs = null;
            if (parameters.Length > 0) {
                var argNames = new HashSet<string>();

                methodArgs = new CodeGenArgumentNode[parameters.Length];
                for (var i = 0; i < parameters.Length; i++) {
                    var currentParam = parameters[i];

                    var type = _namePoolService.GetSanitizedNameString(currentParam->classPrivate->ObjectName);
                    var name = _namePoolService.GetSanitizedNameString(currentParam->ObjectName);

                    methodArgs[i] = new CodeGenArgumentNode {
                        type = type,
                        name = _nameCollisionService.GetNonCollidingName(name, argNames)
                    };
                }
            }

            string[] body;
            if (returnValue is not null) {
                body = new[] {
                    $"{EXECUTING_ADDRESS_FIELD_NAME} = {EXECUTING_ADDRESS_FIELD_NAME}->{nameof(UStruct.superStruct)};",
                    $"return base.{methodName}({string.Join(", ", methodArgs?.Select(x => x.name) ?? [])});"
                };
            }
            else {
                body = new[] {
                    $"{EXECUTING_ADDRESS_FIELD_NAME} = {EXECUTING_ADDRESS_FIELD_NAME}->{nameof(UStruct.superStruct)};",
                    $"base.{methodName}({string.Join(", ", methodArgs?.Select(x => x.name) ?? [])});"
                };
            }

            return new CodeGenMethodNode {
                modifier = modifiers,
                name = methodName,
                attributes = attributes,
                returnType = returnType,
                arguments = methodArgs,
                body = body,
            };
        }
    }
}