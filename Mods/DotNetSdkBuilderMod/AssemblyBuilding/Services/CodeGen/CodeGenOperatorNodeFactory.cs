using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenOperatorNodeFactory : ICodeGenOperatorNodeFactory {
        private readonly ILogger _logger;
        public CodeGenOperatorNodeFactory(ILogger logger) {
            _logger = logger;
        }

        public CodeGenOperatorNode GenerateCastOperator(string castableClassName, string className) {
            var modifiers = $"{PUBLIC}{WHITE_SPACE}{STATIC}{WHITE_SPACE}{EXPLICIT}{WHITE_SPACE}{OPERATOR}";
            var result = $"{NEW}{WHITE_SPACE}{castableClassName}{OPEN_ROUND_BRACKET}{OPERATOR_THIS_CLASS_NAME}{DOT}{ADDRESS_FIELD_NAME}{CLOSED_ROUND_BRACKET}";

            return new CodeGenOperatorNode {
                name = className,
                modifier = modifiers,
                returnType = castableClassName,
                attributes = null,
                result = result,
            };
        }
    }
}