using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenMethodNodeFactory {
        CodeGenMethodNode GenerateOwnedMethodNode(FunctionNode functionNode, Index functionIndex);
        CodeGenMethodNode GenerateInheritedMethodNode(FunctionNode functionNode);
    }
}