using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

public interface IConstructorGenerator {
    void GenerateConstructor(StringBuilder codeBuilder, CodeGenConstructorNode constructorNode);
}