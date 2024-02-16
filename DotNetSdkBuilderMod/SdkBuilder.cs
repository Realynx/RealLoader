﻿using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

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

            // Need to wait for stuff to load into memory
            Thread.Sleep(TimeSpan.FromSeconds(5));

            _sourceCodeGenerator.BuildSourceCode();
        }

        public void Unload() {

        }
    }
}
