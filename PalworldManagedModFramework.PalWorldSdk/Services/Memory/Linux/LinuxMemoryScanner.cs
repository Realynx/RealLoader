using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Linux {
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

        public nint[]? SequenceScan(string signature) {
            var memoryRegions = _memoryMapper.FindMemoryRegions();

            _logger.Debug($"Scanning {memoryRegions.Length} memory regions");
            var foundSequences = _sequenceScanner
                .ScanMemoryRegions(new string[] { signature }, memoryRegions).ToArray();
            _logger.Debug($"Finished scan, found {foundSequences.Length} matches.");

            return foundSequences.FirstOrDefault()?.ToArray();
        }

        public nint[][]? SequenceScan(string[] signature) {
            var memoryRegions = _memoryMapper.FindMemoryRegions();

            _logger.Debug($"Scanning {memoryRegions.Length} memory regions");
            var foundSequences = _sequenceScanner
                .ScanMemoryRegions(signature, memoryRegions);
            _logger.Debug($"Finished scan, found {foundSequences.Length} matches.");

            return foundSequences;
        }

        public nint[]? SequenceScan(string signature, nint startAddress, nint endAddress) {
            return SequenceScan(startAddress, endAddress, signature).FirstOrDefault();
        }

        public nint[][] SequenceScan(nint startAddress, nint endAddress, params string[] signatures) {
            var memoryRegions = _memoryMapper.FindMemoryRegions()
                .Where(i => i.ReadFlag)
                .Where(i => startAddress <= (nint)i.EndAddress && endAddress >= (nint)i.StartAddress);

            _logger.Debug($"Scanning {memoryRegions.Count()} memory regions");
            var foundSequences = _sequenceScanner
                .ScanMemoryRegions(signatures, memoryRegions);
            _logger.Debug($"Finished scan, found {foundSequences.Length} matches.");

            return foundSequences;
        }
    }
}
