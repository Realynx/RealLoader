using Microsoft.Extensions.Logging;

using PalworldManagedModFramework.Services.Detour.Interfaces;
using PalworldManagedModFramework.Services.Detour.Models;

using static PalworldManagedModFramework.Services.Detour.Windows.NativeFunctions;

namespace PalworldManagedModFramework.Services.Detour.Windows {
    public class WindowsMemoryAllocate : IMemoryAllocate {
        private readonly ILogger _logger;

        public WindowsMemoryAllocate(ILogger logger) {
            _logger = logger;
        }

        public nint Allocate(MemoryProtection protection, uint length) {
            var winProtection = protection switch {
                MemoryProtection.Execute => Protection.PAGE_EXECUTE_READWRITE,
                MemoryProtection.ReadWrite => Protection.PAGE_READWRITE,
                MemoryProtection.Readonly => Protection.PAGE_READONLY,
                _ => throw new ArgumentOutOfRangeException(nameof(protection), protection, "Invalid memory protection type.")
            };

            var allocatedMemory = (nint)VirtualAlloc(nint.Zero, length, StateEnum.MEM_COMMIT, winProtection);

            if (allocatedMemory == nint.Zero) {
                _logger.LogError($"Failed to allocate memory. Length: {length}, Protection: {winProtection}");
            }
            else {
                _logger.LogInformation($"Memory allocated. Address: 0x{allocatedMemory:X}, Length: {length}, Protection: {winProtection}");
            }

            return allocatedMemory;
        }

        public bool Free(nint address) {
            var result = VirtualFree(address, 0, StateEnum.MEM_RELEASE);

            if (!result) {
                _logger.LogError($"Failed to free memory at address: 0x{address:X}");
                return false;
            }
            else {
                _logger.LogInformation($"Memory freed at address: 0x{address:X}");
                return true;
            }
        }
    }
}
