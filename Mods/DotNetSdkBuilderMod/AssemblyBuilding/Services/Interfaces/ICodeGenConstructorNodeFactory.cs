using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenConstructorNodeFactory {
        CodeGenConstructorNode GenerateDefaultConstructor(string className);
        CodeGenConstructorNode GenerateCodeGenConstructorNode();
    }
}