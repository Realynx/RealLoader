using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux {
    public class LinuxMemoryScanner : IMemoryScanner {
        private readonly ILogger _logger;
        private readonly ISequenceScanner _sequenceScanner;
        private readonly IMemoryMapper _memoryMapper;

        public LinuxMemoryScanner(ILogger logger, ISequenceScanner sequenceScanner,
            IMemoryMapper memoryMapper) {
            _logger = logger;
            _sequenceScanner = sequenceScanner;
            _memoryMapper = memoryMapper;
        }

        public nint? SingleSequenceScan(string signature) {
            return SequenceScan(signature).FirstOrDefault();
        }

        public nint[] SequenceScan(string signature) {
            var memoryRegions = _memoryMapper.FindMemoryRegions();
            _logger.Debug($"Scanning {memoryRegions.Length} memory regions");

            var foundSequences = _sequenceScanner
                .ScanMemoryRegions(signature, memoryRegions).ToArray();
            _logger.Debug($"Finished scan, found {foundSequences.Length} matches.");

            return foundSequences;
        }

        public nint[] SequenceScan(string signature, nint startAddress, nint endAddress) {
            var memoryRegions = _memoryMapper.FindMemoryRegions()
                .Where(i => i.ReadFlag)
                .Where(i => startAddress <= (nint)i.EndAddress && endAddress >= (nint)i.StartAddress);

            _logger.Debug($"Scanning {memoryRegions.Count()} memory regions");
            var foundSequences = _sequenceScanner
                .ScanMemoryRegions(signature, memoryRegions).ToArray();
            _logger.Debug($"Finished scan, found {foundSequences.Length} matches.");

            return foundSequences;
        }
    }
}
