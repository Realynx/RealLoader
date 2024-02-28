using DotNetSdkBuilderMod.AssemblyBuilding.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IReflectedGraphBuilder {
        ClassNode? BuildRootNode();
    }
}