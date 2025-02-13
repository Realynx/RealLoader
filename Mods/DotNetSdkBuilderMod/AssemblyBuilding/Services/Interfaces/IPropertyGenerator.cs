using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IPropertyGenerator {
        void GenerateProperty(StringBuilder codeBuilder, CodeGenPropertyNode propertyNode);
    }
}