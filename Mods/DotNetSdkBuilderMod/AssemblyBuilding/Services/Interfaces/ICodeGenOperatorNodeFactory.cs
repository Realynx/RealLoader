using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenOperatorNodeFactory {
        CodeGenOperatorNode GenerateCastOperator(string castableClassName, string className);
    }
}