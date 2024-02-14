using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod {
    [PalworldMod(nameof(SdkBuilder), "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class SdkBuilder : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly ISourceCodeGenerator _sourceCodeGenerator;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger, ISourceCodeGenerator sourceCodeGenerator) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _sourceCodeGenerator = sourceCodeGenerator;
        }

        public unsafe void Load() {
            _logger.Debug("Loading SDK Builder!");
            _sourceCodeGenerator.BuildSourceCode();
        }


        //private unsafe string GetFullPath(UObjectBase uObjectBase) {
        //    var nameBuilder = new List<string>();
        //    for (UObjectBase* currentObj = &uObjectBase; currentObj is not null; currentObj = (UObjectBase*)currentObj->outerPrivate) {
        //        var baseName = _globalObjects.GetNameString(currentObj->namePrivate.comparisonIndex);
        //        nameBuilder.Add(baseName);
        //    }

        //    nameBuilder.Reverse();
        //    return string.Join(".", nameBuilder);
        //}



        static string[] GetFlagNames(Enum flags) {
            var flagNames = new List<string>();

            foreach (Enum value in Enum.GetValues(flags.GetType())) {
                if (flags.HasFlag(value)) {
                    if (!value.Equals(default(Enum))) {
                        flagNames.Add(Enum.GetName(flags.GetType(), value));
                    }
                }
            }

            return flagNames.ToArray();
        }

        public void Unload() {

        }
    }
}
