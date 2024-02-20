using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Windows {
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

        public nint? SingleSequenceScan(ByteCodePattern byteCodePattern) {
            return SequenceScan(byteCodePattern).FirstOrDefault();
        }

        public nint[][]? SequenceScan(params ByteCodePattern[] byteCodePatterns) {
            var processModule = Process.GetCurrentProcess().MainModule!;

            var endAddress = processModule.BaseAddress + processModule.ModuleMemorySize;
            return SequenceScan(_baseAddress, endAddress, byteCodePatterns);
        }

        public nint[]? SequenceScan(ByteCodePattern byteCodePattern) {
            var processModule = Process.GetCurrentProcess().MainModule!;

            var endAddress = processModule.BaseAddress + processModule.ModuleMemorySize;
            return SequenceScan(byteCodePattern, _baseAddress, endAddress);
        }

        public nint[]? SequenceScan(ByteCodePattern byteCodePattern, nint startAddress, nint endAddress) {
            return SequenceScan(startAddress, endAddress, byteCodePattern).FirstOrDefault();
        }

        public nint[][] SequenceScan(nint startAddress, nint endAddress, params ByteCodePattern[] byteCodePatterns) {
            _processSuspender.PauseSelf();

            var memoryRegions = _memoryMapper.FindMemoryRegions()
                .Where(i => i.ReadFlag)
                .Where(i => startAddress <= (nint)i.EndAddress && endAddress >= (nint)i.StartAddress);

            var foundSequences = _sequenceScanner.ScanMemoryRegions(byteCodePatterns, memoryRegions);

            _processSuspender.ResumeSelf();

            return foundSequences;
        }
    }
}
