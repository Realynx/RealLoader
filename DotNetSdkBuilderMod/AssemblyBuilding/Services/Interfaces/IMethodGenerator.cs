using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IMethodGenerator {
        void GenerateMethod(StringBuilder codeBuilder, CodeGenMethodNode methodNode);
    }
}