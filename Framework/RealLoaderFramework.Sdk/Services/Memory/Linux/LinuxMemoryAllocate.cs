using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Versioning;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Models;

using static RealLoaderFramework.Sdk.Services.Memory.Linux.NativeFunctions;


namespace RealLoaderFramework.Sdk.Services.Memory.Linux {
    [SupportedOSPlatform("linux")]
    public class LinuxMemoryAllocate : IMemoryAllocate {
        private readonly ILogger _logger;
        private readonly IMemoryMapper _memoryMapper;

        private readonly ConcurrentDictionary<nint, nuint> _mappedAddresses = new();

        public LinuxMemoryAllocate(ILogger logger, IMemoryMapper memoryMapper) {
            _logger = logger;
            _memoryMapper = memoryMapper;
        }

        public nint Allocate(SimpleMemoryProtection protection, uint length) {
            var linuxProtection = ConvertToMProtection(protection);
            var allocatedMemory = MemoryMap(nint.Zero, length, linuxProtection, MMapFlags.MAP_SHARED | MMapFlags.MAP_ANONYMOUS, -1, 0);

            if (allocatedMemory == -1) {
                _logger.Error($"Failed to allocate memory. Length: {length}, Protection: {linuxProtection}");
                return nint.Zero;
            }
            else {
                if (!_mappedAddresses.TryAdd(allocatedMemory, length)) {
                    throw new UnreachableException($"Failed to record 0x{allocatedMemory:X} as a mapped address. Already exists: {_mappedAddresses.ContainsKey(allocatedMemory)}.");
                }

                _logger.Info($"Memory allocated. Address: 0x{allocatedMemory:X}, Length: {length}, Protection: {linuxProtection}");
                return allocatedMemory;
            }
        }

        public bool Free(nint address) {
            if (!_mappedAddresses.TryRemove(address, out var length)) {
                _logger.Error($"Tried to free memory that was not mapped by us. Address: 0x{address:X}.");
                return false;
            }

            var result = MemoryUnmap(address, length);

            if (result == -1) {
                _logger.Error($"Failed to free memory at address: 0x{address:X}");
                return false;
            }
            else if (result == 0) {
                _logger.Info($"Memory freed at address: 0x{address:X}");
                return true;
            }
            else {
                _logger.Warning($"Unexpected result from munmap when freeing 0x{address:X}: {result}. Assuming success.");
                return true;
            }
        }

        public bool SetProtection(nint address, uint length, SimpleMemoryProtection protection, out SimpleMemoryProtection previousProtection) {
            var pageAlignedAddress = address & ~0xfff;
            var linuxProtection = ConvertToMProtection(protection);

            var previousMProtect = GetProtection(address);

            var result = MemoryProtect(pageAlignedAddress, 0x1000, linuxProtection);

            if (result == -1) {
                _logger.Error($"Failed to set protection for address: 0x{pageAlignedAddress:X}");
                previousProtection = SimpleMemoryProtection.None;
                return false;
            }
            else if (result == 0) {
                _logger.Info($"Set protection of 0x{pageAlignedAddress:X} to {protection}.");
                previousProtection = ConvertToMemoryProtection(previousMProtect);
                return true;
            }
            else {
                _logger.Warning($"Unexpected result from mmap when setting protection for 0x{pageAlignedAddress:X}: {result}. Assuming success.");
                previousProtection = ConvertToMemoryProtection(previousMProtect);
                return true;
            }
        }

        private MProtectProtect GetProtection(nint address) {
            var memoryRegion = _memoryMapper.FindMemoryRegions()
                .FirstOrDefault(i => i.StartAddress <= (ulong)address && i.EndAddress >= (ulong)address);

            var memoryProtection = MProtectProtect.PROT_NONE;

            if (memoryRegion.ReadFlag) {
                memoryProtection |= MProtectProtect.PROT_READ;
            }

            if (memoryRegion.WriteFlag) {
                memoryProtection |= MProtectProtect.PROT_WRITE;
            }

            if (memoryRegion.ExecuteFlag) {
                memoryProtection |= MProtectProtect.PROT_EXEC;
            }

            return memoryProtection;
        }
    }
}
