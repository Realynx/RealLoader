using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenInterfaceNodeFactory {
        CodeGenInterfaceNode GenerateICreatableUObject(string className);
    }
}