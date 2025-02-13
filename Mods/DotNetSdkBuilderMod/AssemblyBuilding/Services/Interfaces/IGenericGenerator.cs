using System.Text;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IGenericGenerator {
        void GenerateGenerics(StringBuilder codeBuilder, string[] genericTypes);
    }
}