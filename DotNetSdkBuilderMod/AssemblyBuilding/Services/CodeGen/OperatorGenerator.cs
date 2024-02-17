using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class OperatorGenerator : IOperatorGenerator {
        public void GenerateOperator(StringBuilder codeBuilder, CodeGenOperatorNode operatorNode) {
            codeBuilder.AppendIndented(operatorNode.modifier, 2);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(operatorNode.returnType);

            codeBuilder.Append(OPEN_ROUND_BRACKET);
            codeBuilder.Append(operatorNode.name);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(OPERATOR_THIS_CLASS_NAME);
            codeBuilder.Append(CLOSED_ROUND_BRACKET);

            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(LAMBDA);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(operatorNode.result);
            codeBuilder.AppendLine(SEMICOLON);
        }
    }
}