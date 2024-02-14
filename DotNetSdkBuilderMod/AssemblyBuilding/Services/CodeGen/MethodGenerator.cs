using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class MethodGenerator : IMethodGenerator {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;
        private readonly UnrealReflection _unrealReflection;

        public MethodGenerator(ILogger logger, IGlobalObjects globalObjects, UnrealReflection unrealReflection) {
            _logger = logger;
            _globalObjects = globalObjects;
            _unrealReflection = unrealReflection;
        }

        public unsafe void GenerateMethod(StringBuilder codeBuilder, UFunction method) {
            var modifiers = GetMethodModifiers(method);

            codeBuilder.AppendIndented(modifiers, 2);
            codeBuilder.Append(WHITE_SPACE);

            var signature = _unrealReflection.GetFunctionSignature(method);
            if (signature.returnValue.HasValue) {
                var signatureName = signature.returnValue.Value.classPrivate->ObjectName;
                var signatureString = _globalObjects.GetNameString(signatureName);
                codeBuilder.Append(signatureString);
            }
            else {
                codeBuilder.Append(VOID);
            }
            codeBuilder.Append(WHITE_SPACE);

            var methodName = _globalObjects.GetNameString(method.baseUstruct.ObjectName);

            codeBuilder.Append(methodName);

            var parameters = string.Join($"{COMMA}{WHITE_SPACE}", signature.parameters.Select(param => {
                var propName = _globalObjects.GetNameString(param.ObjectName);
                var className = _globalObjects.GetNameString(param.classPrivate->ObjectName);

                return $"{className}{WHITE_SPACE}{propName}";
            }));

            codeBuilder.Append(OPEN_ROUND_BRACKET);
            codeBuilder.Append(parameters);
            codeBuilder.Append(CLOSED_ROUND_BRACKET);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(OPEN_CURLY_BRACKET);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(CLOSED_CURLY_BRACKET);
        }

        private string GetMethodModifiers(UFunction method) {
            // TODO: At some point we may want to get more details here.
            return PUBLIC;
        }
    }
}