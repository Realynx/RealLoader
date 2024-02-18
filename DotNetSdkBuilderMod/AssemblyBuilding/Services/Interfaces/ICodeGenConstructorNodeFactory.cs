using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenConstructorNodeFactory {
        CodeGenConstructorNode GenerateDefaultConstructor(ClassNode classNode, string className);
        CodeGenConstructorNode GenerateCodeGenConstructorNode();
    }
}