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

        private readonly nint _baseAddress;

        public WindowsMemoryScanner(ILogger logger, ISequenceScanner sequenceScanner,
            IProcessSuspender processSuspender, IMemoryMapper memoryMapper) {
            _logger = logger;
            _sequenceScanner = sequenceScanner;
            _processSuspender = processSuspender;
            _memoryMapper = memoryMapper;

            var processModule = Process.GetCurrentProcess().MainModule!;
            _baseAddress = processModule.BaseAddress;

            _logger.Debug($"Windows Main Module Base Address: 0x{_baseAddress:X}");
        }

        public nint? SingleSequenceScan(string signature) {
            return SequenceScan(signature).FirstOrDefault();
        }

        public nint[] SequenceScan(string signature) {
            var processModule = Process.GetCurrentProcess().MainModule!;

            var endAddress = processModule.BaseAddress + processModule.ModuleMemorySize;
            return SequenceScan(signature, _baseAddress, endAddress);
        }

        public nint[] SequenceScan(string signature, nint startAddress, nint endAddress) {
            _processSuspender.PauseSelf();

            var memoryRegions = _memoryMapper.FindMemoryRegions()
                .Where(i => i.ReadFlag)
                .Where(i => startAddress <= (nint)i.EndAddress && endAddress >= (nint)i.StartAddress);
            var foundSequences = _sequenceScanner.ScanMemoryRegions(signature, memoryRegions);

            _processSuspender.ResumeSelf();

            return foundSequences.ToArray();
        }
    }
}
