using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

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

        public unsafe CodeGenMethodNode GenerateCodeGenMethodNode(UFunction* method, Index methodIndex) {
            var modifiers = PUBLIC;

            var nonSanitizedMethodName = _namePoolService.GetNameString(method->baseUstruct.ObjectName);
            var methodName = _namePoolService.GetSanitizedNameString(method->baseUstruct.ObjectName);
            if (char.IsDigit(methodName[0])) {
                methodName = $"_{methodName}";
            }

            var attributes = new [] {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"{QUOTE}{nonSanitizedMethodName}{QUOTE}")
            };

            var parameters = _unrealReflection.GetFunctionSignature(method, out var returnValue, out var returnValueIndex);
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
                        $"{THIS}{DOT}{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}{OPEN_ROUND_BRACKET}{nameof(UObjectInterop.GetFunctionStructPointer)}{OPEN_ROUND_BRACKET}{methodIndex}{CLOSED_ROUND_BRACKET}{COMMA}{WHITE_SPACE}{string.Join($"{COMMA}{WHITE_SPACE}", methodArgs.Select(x => x.name))}{CLOSED_ROUND_BRACKET}{SEMICOLON}",
                    };
                }
                else {
                    var sb = new StringBuilder($"{THIS}{DOT}{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}{OPEN_ROUND_BRACKET}{nameof(UObjectInterop.GetFunctionStructPointer)}{OPEN_ROUND_BRACKET}{methodIndex}{CLOSED_ROUND_BRACKET}");
                    var argsBeforeReturnValue = methodArgs.Take(returnValueIndex.Value).Select(x => x.name).ToArray();
                    if (argsBeforeReturnValue.Length > 0) {
                        sb.Append($"{COMMA}{WHITE_SPACE}");
                        sb.Append(string.Join($"{COMMA}{WHITE_SPACE}", argsBeforeReturnValue));
                    }

                    sb.Append($"{COMMA}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}");

                    var argsAfterReturnValue = methodArgs.Skip(returnValueIndex.Value).Select(x => x.name).ToArray();
                    if (argsAfterReturnValue.Length > 0) {
                        sb.Append($"{COMMA}{WHITE_SPACE}");
                        sb.Append(string.Join($"{COMMA}{WHITE_SPACE}", argsAfterReturnValue));
                    }

                    sb.Append(CLOSED_ROUND_BRACKET);
                    sb.Append(SEMICOLON);

                    body = new[] {
                        $"{returnType}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{WHITE_SPACE}{EQUALS}{WHITE_SPACE}{DEFAULT}{SEMICOLON}",
                        sb.ToString(),
                        $"{RETURN}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{SEMICOLON}",
                    };
                }
            }
            else {
                if (returnValue is null) {
                    body = new[] {
                        $"{THIS}{DOT}{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}{OPEN_ROUND_BRACKET}{nameof(UObjectInterop.GetFunctionStructPointer)}{OPEN_ROUND_BRACKET}{methodIndex}{CLOSED_ROUND_BRACKET}{CLOSED_ROUND_BRACKET}{SEMICOLON}",
                    };
                }
                else {
                    body = new[] {
                        $"{returnType}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{WHITE_SPACE}{EQUALS}{WHITE_SPACE}{DEFAULT}{SEMICOLON}",
                        $"{THIS}{DOT}{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}{OPEN_ROUND_BRACKET}{nameof(UObjectInterop.GetFunctionStructPointer)}{OPEN_ROUND_BRACKET}{methodIndex}{CLOSED_ROUND_BRACKET}{COMMA}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{CLOSED_ROUND_BRACKET}{SEMICOLON}",
                        $"{RETURN}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{SEMICOLON}",
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

        public unsafe CodeGenMethodNode GenerateInheritedMethod(UFunction* method) {
            var modifiers = $"{PUBLIC}{WHITE_SPACE}{NEW}";

            var nonSanitizedMethodName = _namePoolService.GetNameString(method->baseUstruct.ObjectName);
            var methodName = _namePoolService.GetSanitizedNameString(method->baseUstruct.ObjectName);
            methodName = _nameCollisionService.GetNonCollidingName(methodName);
            if (char.IsDigit(methodName[0])) {
                methodName = $"_{methodName}";
            }

            var attributes = new [] {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"{QUOTE}{nonSanitizedMethodName}{QUOTE}"),
                _attributeNodeFactory.GenerateAttribute(COMPILER_GENERATED_ATTRIBUTE)
            };

            var parameters = _unrealReflection.GetFunctionSignature(method, out var returnValue, out var returnValueIndex);
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
                    $"{EXECUTING_ADDRESS_FIELD_NAME}{WHITE_SPACE}{EQUALS}{WHITE_SPACE}{EXECUTING_ADDRESS_FIELD_NAME}{ARROW}{nameof(UStruct.superStruct)}{SEMICOLON}",
                    $"{RETURN}{WHITE_SPACE}{BASE}{DOT}{methodName}{OPEN_ROUND_BRACKET}{string.Join($"{COMMA}{WHITE_SPACE}", methodArgs?.Select(x => x.name) ?? Enumerable.Empty<string>())}{CLOSED_ROUND_BRACKET}{SEMICOLON}"
                };
            }
            else {
                body = new[] {
                    $"{EXECUTING_ADDRESS_FIELD_NAME}{WHITE_SPACE}{EQUALS}{WHITE_SPACE}{EXECUTING_ADDRESS_FIELD_NAME}{ARROW}{nameof(UStruct.superStruct)}{SEMICOLON}",
                    $"{BASE}{DOT}{methodName}{OPEN_ROUND_BRACKET}{string.Join($"{COMMA}{WHITE_SPACE}", methodArgs?.Select(x => x.name) ?? Enumerable.Empty<string>())}{CLOSED_ROUND_BRACKET}{SEMICOLON}"
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