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
            var modifiers = "public static explicit operator";
            var result = $"new {castableClassName}({OPERATOR_THIS_CLASS_NAME}.{ADDRESS_FIELD_NAME})";

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