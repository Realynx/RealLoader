using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IArgumentGenerator {
        void GenerateArguments(StringBuilder codeBuilder, CodeGenArgumentNode[] argumentNodes);
    }
}