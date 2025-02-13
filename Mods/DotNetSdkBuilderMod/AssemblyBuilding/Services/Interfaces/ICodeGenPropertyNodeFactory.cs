using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenPropertyNodeFactory {
        CodeGenPropertyNode GenerateOwnedPropertyNode(PropertyNode propertyNode);
        CodeGenPropertyNode GenerateInheritedPropertyNode(PropertyNode propertyNode);
    }
}