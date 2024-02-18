using Microsoft.Extensions.Logging;

using PalworldManagedModFramework.Services.Detour.Models;

using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Services.Detour.Interfaces;

using static PalworldManagedModFramework.Services.Detour.Linux.NativeFunctions;


namespace PalworldManagedModFramework.Services.Detour.Linux {
    public class LinuxMemoryAllocate : IMemoryAllocate {
        private readonly ILogger _logger;

        public LinuxMemoryAllocate(ILogger logger) {
            _logger = logger;
        }

        public nint Allocate(MemoryProtection protection, uint length) {
            var linuxProtection = protection switch {
                MemoryProtection.Execute => MProtectProtect.PROT_EXEC | MProtectProtect.PROT_READ | MProtectProtect.PROT_WRITE,
                MemoryProtection.ReadWrite => MProtectProtect.PROT_READ | MProtectProtect.PROT_WRITE,
                MemoryProtection.Readonly => MProtectProtect.PROT_READ,
                _ => throw new ArgumentOutOfRangeException(nameof(protection), protection, "Invalid memory protection type.")
            };

            var mapFlags = MMapFlags.MAP_SHARED | MMapFlags.MAP_ANONYMOUS;
            MemoryMap(nint.Zero, length, linuxProtection, mapFlags, -1, 0);
        }


        public bool Free(nint address) {

        }

        public MemoryProtection AllowExecute(nint startAddress, nint endAddress) {
            var length = endAddress - startAddress;
            var result = LinuxNativeMethods.MemoryProtect(startAddress, (nuint)length, LinuxStructs.MProtectProtect.PROT_EXEC);

            throw new NotImplementedException();
        }

        public MemoryProtection SetProtection(MemoryProtection memoryProtection, nint startAddress, nint endAddress) {
            var length = endAddress - startAddress;
            LinuxNativeMethods.MemoryProtect(startAddress, (nuint)length, );
            throw new NotImplementedException();
        }
    }
}
