using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class OperatorGenerator : IOperatorGenerator {
        private readonly ILogger _logger;

        public OperatorGenerator(ILogger logger) {
            _logger = logger;
        }

        public void GenerateOperator(StringBuilder codeBuilder, CodeGenOperatorNode operatorNode) {
            codeBuilder.AppendIndent(2);
            codeBuilder.Append($"{operatorNode.modifier} {operatorNode.returnType}({operatorNode.name} {OPERATOR_THIS_CLASS_NAME}) => {operatorNode.result};");
        }
    }
}