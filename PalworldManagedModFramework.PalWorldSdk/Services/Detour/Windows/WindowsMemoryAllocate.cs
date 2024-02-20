using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;

using static PalworldManagedModFramework.Sdk.Services.Detour.Windows.NativeFunctions;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Windows {
    public class WindowsMemoryAllocate : IMemoryAllocate {
        private readonly ILogger _logger;

        public WindowsMemoryAllocate(ILogger logger) {
            _logger = logger;
        }

        public nint Allocate(MemoryProtection protection, uint length) {
            var winProtection = ConvertToProtection(protection);

            var allocatedMemory = VirtualAlloc(nint.Zero, length, StateEnum.MEM_COMMIT, winProtection);

            if (allocatedMemory == nint.Zero) {
                _logger.Error($"Failed to allocate memory. Length: {length}, Protection: {winProtection}");
            }
            else {
                _logger.Info($"Memory allocated. Address: 0x{allocatedMemory:X}, Length: {length}, Protection: {winProtection}");
            }

            return allocatedMemory;
        }

        public bool Free(nint address) {
            var result = VirtualFree(address, 0, StateEnum.MEM_RELEASE);

            if (!result) {
                _logger.Error($"Failed to free memory at address: 0x{address:X}");
                return false;
            }

            _logger.Info($"Memory freed at address: 0x{address:X}");
            return true;
        }

        public unsafe bool SetProtection(nint address, uint length, MemoryProtection protection, out MemoryProtection previousProtection) {
            var winProtection = ConvertToProtection(protection);

            var result = VirtualProtect(address, length, winProtection, out var oldProtection);

            if (!result || oldProtection == 0) {
                _logger.Error($"Failed to set memory protection at address: 0x{address:X}");
                previousProtection = 0;
                return false;
            }

            _logger.Info($"Set memory protection to {protection} at address: 0x{address:X}");
            previousProtection = ConvertToMemoryProtection((Protection)oldProtection);
            return true;
        }

        private Protection ConvertToProtection(MemoryProtection memoryProtection) {
            return memoryProtection switch {
                MemoryProtection.Execute => Protection.PAGE_EXECUTE_READWRITE,
                MemoryProtection.ReadWrite => Protection.PAGE_READWRITE,
                MemoryProtection.Readonly => Protection.PAGE_READONLY,
                _ => throw new ArgumentOutOfRangeException(nameof(memoryProtection), memoryProtection, "Invalid memory protection type.")
            };
        }

        private MemoryProtection ConvertToMemoryProtection(Protection protection) {
            return protection switch {
                Protection.PAGE_EXECUTE_READWRITE => MemoryProtection.Execute,
                Protection.PAGE_EXECUTE_READ => MemoryProtection.Execute,
                Protection.PAGE_READWRITE => MemoryProtection.ReadWrite,
                Protection.PAGE_READONLY => MemoryProtection.Readonly,
                _ => throw new ArgumentOutOfRangeException(nameof(protection), protection, "Invalid memory protection type.")
            };
        }
    }
}
