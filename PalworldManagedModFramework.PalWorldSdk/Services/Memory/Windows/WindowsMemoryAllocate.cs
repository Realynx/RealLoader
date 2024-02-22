using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

using static PalworldManagedModFramework.Sdk.Services.Memory.Windows.NativeFunctions;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Windows {
    public class WindowsMemoryAllocate : IMemoryAllocate {
        private readonly ILogger _logger;

        public WindowsMemoryAllocate(ILogger logger) {
            _logger = logger;
        }

        public nint Allocate(SimpleMemoryProtection protection, uint length) {
            var winProtection = ConvertToProtection(protection);
            var allocatedMemory = VirtualAlloc(nint.Zero, length, PageState.MEM_COMMIT, winProtection);

            if (allocatedMemory == nint.Zero) {
                _logger.Error($"Failed to allocate memory. Length: {length}, Protection: {winProtection}");
            }
            else {
                _logger.Debug($"Memory allocated. Address: 0x{allocatedMemory:X}, Length: {length}, Protection: {winProtection}");
            }

            return allocatedMemory;
        }

        public bool Free(nint address) {
            var result = VirtualFree(address, 0, PageState.MEM_RELEASE);

            if (!result) {
                _logger.Error($"Failed to free memory at address: 0x{address:X}");
                return false;
            }

            _logger.Debug($"Memory freed at address: 0x{address:X}");
            return true;
        }

        public unsafe bool SetProtection(nint address, uint length, SimpleMemoryProtection protection, out SimpleMemoryProtection previousProtection) {
            var winProtection = ConvertToProtection(protection);
            var result = VirtualProtect(address, length, winProtection, out var oldProtection);

            if (!result || oldProtection == 0) {
                _logger.Error($"Failed to set memory protection at address: 0x{address:X}");
                previousProtection = 0;
                return false;
            }

            _logger.Debug($"Set memory protection to {protection} at address: 0x{address:X}");
            previousProtection = ConvertToMemoryProtection((Protection)oldProtection);
            return true;
        }
    }
}
