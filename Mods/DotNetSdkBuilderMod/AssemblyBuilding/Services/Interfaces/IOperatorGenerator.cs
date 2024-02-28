using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

public interface IOperatorGenerator {
    void GenerateOperator(StringBuilder codeBuilder, CodeGenOperatorNode operatorNode);
}