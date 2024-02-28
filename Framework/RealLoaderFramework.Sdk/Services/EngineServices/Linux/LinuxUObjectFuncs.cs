using System.Runtime.Versioning;

using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

using static RealLoaderFramework.Sdk.Services.EngineServices.Interfaces.IUObjectFuncs;

namespace RealLoaderFramework.Sdk.Services.EngineServices.Linux {
    [SupportedOSPlatform("linux")]
    public class LinuxUObjectFuncs : IUObjectFuncs {
        [MachineCodePattern("53 66 2E 0F 1F 84 00 00 00 00 ? 0F 1F 44 00 ? 48 ? ? 48 8B ? ? 48 ? ? 74 ? F6 43 ? ? 74 ?", PatternType.Function)]
        public UObjectBaseUtility_GetOutermost GetParentPackage { get; set; } = null!;
    }
}
