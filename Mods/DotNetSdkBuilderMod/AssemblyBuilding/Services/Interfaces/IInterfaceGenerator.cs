using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IInterfaceGenerator {
        void GenerateInterfaces(StringBuilder codeBuilder, CodeGenInterfaceNode[] interfaceNodes);
    }
}