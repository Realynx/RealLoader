using System.Diagnostics.CodeAnalysis;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface ICodeGenAttributeNodeFactory {
        CodeGenAttributeNode GenerateAssemblyAttribute([ConstantExpected] string attributeName, string attributeValue);
        CodeGenAttributeNode GenerateAttribute([ConstantExpected] string attributeName, string attributeValue);
        CodeGenAttributeNode GenerateAttribute([ConstantExpected] string attributeName);
    }
}