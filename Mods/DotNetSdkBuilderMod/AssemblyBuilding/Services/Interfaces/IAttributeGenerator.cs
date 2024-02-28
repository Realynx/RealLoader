using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IAttributeGenerator {
        void GenerateAttribute(StringBuilder codeBuilder, CodeGenAttributeNode attributeNode, int indent);
    }
}