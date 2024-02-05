using System.Diagnostics;

using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows {
    public class WindowsMemoryScanner : IMemoryScanner {
        private readonly ILogger _logger;
        private readonly ISequenceScanner _sequenceScanner;
        private readonly IProcessSuspender _processSuspender;
        private readonly IMemoryMapper _memoryMapper;

        public WindowsMemoryScanner(ILogger logger, ISequenceScanner sequenceScanner,
            IProcessSuspender processSuspender, IMemoryMapper memoryMapper) {
            _logger = logger;
            _sequenceScanner = sequenceScanner;
            _processSuspender = processSuspender;
            _memoryMapper = memoryMapper;
        }

        public nint? SingleSequenceScan(string signature) {
            return SequenceScan(signature).FirstOrDefault();
        }

        public nint[] SequenceScan(string signature) {
            var processModule = Process.GetCurrentProcess().MainModule!;

            var startAddress = processModule.BaseAddress;
            var endAddress = processModule.BaseAddress + processModule.ModuleMemorySize;

            _logger.Debug($"Windows Main Module Address Space: 0x{startAddress:X} - 0x{endAddress:X}");
            return SequenceScan(signature, startAddress, endAddress);
        }

        public nint[] SequenceScan(string signature, nint startAddress, nint endAddress) {
            _processSuspender.PauseSelf();

            var memoryRegions = _memoryMapper.FindMemoryRegions()
                .Where(i => i.ReadFlag)
                .Where(i => startAddress <= (nint)i.EndAddress && endAddress >= (nint)i.StartAddress);

            _logger.Debug($"Scanning {memoryRegions.Count()} memory regions");
            var foundSequences = _sequenceScanner.ScanMemoryRegions(signature, memoryRegions);
            _logger.Debug($"Finished scan, found {foundSequences.Count()} matches.");

            _processSuspender.ResumeSelf();
            return foundSequences.ToArray();
        }
    }
}
