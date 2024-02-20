using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Interfaces;

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

        public unsafe CodeGenMethodNode GenerateCodeGenMethodNode(UFunction* method) {
            string modifiers;
            if (method->functionFlags.HasFlag(EFunctionFlags.FUNC_Static)) {
                modifiers = $"{PUBLIC}{WHITE_SPACE}{STATIC}";
            }
            else {
                modifiers = $"{PUBLIC}";
            }

            var methodName = _namePoolService.GetNameString(method->baseUstruct.ObjectName);

            CodeGenAttributeNode[]? attributes = null;

            var parameters = _unrealReflection.GetFunctionSignature(method, out var returnValue);
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

            return new CodeGenMethodNode {
                modifier = modifiers,
                name = methodName,
                attributes = attributes,
                returnType = returnType,
                arguments = methodArgs
            };
        }
    }
}