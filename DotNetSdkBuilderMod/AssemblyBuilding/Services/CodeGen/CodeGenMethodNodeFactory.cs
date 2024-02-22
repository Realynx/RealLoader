using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenMethodNodeFactory : ICodeGenMethodNodeFactory {
        private readonly ILogger _logger;
        private readonly INamePoolService _namePoolService;
        private readonly IUnrealReflection _unrealReflection;
        public CodeGenMethodNodeFactory(ILogger logger, INamePoolService namePoolService, IUnrealReflection unrealReflection) {
            _logger = logger;
            _namePoolService = namePoolService;
            _unrealReflection = unrealReflection;
        }

        public unsafe CodeGenMethodNode GenerateCodeGenMethodNode(UFunction* method, Index methodIndex) {
            string modifiers;
            if (method->functionFlags.HasFlag(EFunctionFlags.FUNC_Static)) {
                modifiers = $"{PUBLIC}{WHITE_SPACE}{STATIC}";
            }
            else {
                modifiers = $"{PUBLIC}";
            }

            var methodName = _namePoolService.GetNameString(method->baseUstruct.ObjectName);

            CodeGenAttributeNode[]? attributes = null;

            var parameters = _unrealReflection.GetFunctionSignature(method, out var returnValue, out var returnValueIndex);
            string returnType;
            if (returnValue is not null) {
                var signatureName = returnValue->classPrivate->ObjectName;
                var signatureString = _namePoolService.GetNameString(signatureName);
                returnType = signatureString;
            }
            else {
                returnType = VOID;
            }

            CodeGenArgumentNode[]? methodArgs = null;
            if (parameters.Length > 0) {
                methodArgs = new CodeGenArgumentNode[parameters.Length];
                for (var i = 0; i < parameters.Length; i++) {
                    var currentParam = parameters[i];

                    var type = _namePoolService.GetNameString(currentParam->classPrivate->ObjectName);
                    var name = _namePoolService.GetNameString(currentParam->ObjectName);

                    methodArgs[i] = new CodeGenArgumentNode {
                        type = type,
                        name = name
                    };
                }
            }

            string[] body;
            if (methodArgs is not null) {
                if (returnValue is null) {
                    body = new[] {
                        $"{THIS}.{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}{OPEN_ROUND_BRACKET}{nameof(UObjectInterop.GetFunctionStructPointer)}{OPEN_ROUND_BRACKET}{methodIndex}{CLOSED_ROUND_BRACKET}{COMMA}{WHITE_SPACE}{string.Join($"{COMMA}{WHITE_SPACE}", methodArgs.Select(x => x.name))}{CLOSED_ROUND_BRACKET}{SEMICOLON}",
                    };
                }
                else {
                    var bodyStart = $"{THIS}.{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}{OPEN_ROUND_BRACKET}{nameof(UObjectInterop.GetFunctionStructPointer)}{OPEN_ROUND_BRACKET}{methodIndex}{CLOSED_ROUND_BRACKET}{COMMA}{WHITE_SPACE}{string.Join($"{COMMA}{WHITE_SPACE}", methodArgs.Take(returnValueIndex.Value - 1).Select(x => x.name))}";
                    var bodyEnd = $"{string.Join($"{COMMA}{WHITE_SPACE}", methodArgs.Skip(returnValueIndex.Value).Select(x => x.name))}{CLOSED_ROUND_BRACKET}";
                    body = new[] {
                        $"{returnType}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{SEMICOLON}",
                        $"{bodyStart}{COMMA}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{COMMA}{WHITE_SPACE}{bodyEnd}{SEMICOLON}",
                        $"{RETURN}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{SEMICOLON}",
                    };
                }
            }
            else {
                if (returnValue is null) {
                    body = new[] {
                        $"{THIS}.{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}{OPEN_ROUND_BRACKET}{nameof(UObjectInterop.GetFunctionStructPointer)}{OPEN_ROUND_BRACKET}{methodIndex}{CLOSED_ROUND_BRACKET}{CLOSED_ROUND_BRACKET}{SEMICOLON}",
                    };
                }
                else {
                    body = new[] {
                        $"{returnType}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{SEMICOLON}",
                        $"{THIS}.{CODE_GEN_INTEROP_INVOKE_METHOD_NAME}{OPEN_ROUND_BRACKET}{nameof(UObjectInterop.GetFunctionStructPointer)}{OPEN_ROUND_BRACKET}{methodIndex}{CLOSED_ROUND_BRACKET}{COMMA}{WHITE_SPACE}{CODE_GEN_INTEROP_RETURN_VALUE_NAME}{CLOSED_ROUND_BRACKET}{SEMICOLON}",
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
    }
}