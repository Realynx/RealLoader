using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Models;

using static RealLoaderFramework.Sdk.Services.Memory.Windows.NativeFunctions;

namespace RealLoaderFramework.Sdk.Services.Memory.Windows {
    [SupportedOSPlatform("windows")]
    public class WindowsMemoryMapper : IMemoryMapper {
        public MemoryRegion[] FindMemoryRegions() {
            var currentProcess = Process.GetCurrentProcess();
            var currentProcessHandle = currentProcess.Handle;

            return EnumerateMemoryRegions(currentProcessHandle, 0).ToArray();
        }

        public nint GetBaseAddress() {
            return Process.GetCurrentProcess().MainModule!.BaseAddress;
        }

        public SimpleMemoryProtection GetProtection(nint address) {
            throw new NotImplementedException("This is a linux function. Windows has native function for getting memory protection!");
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private unsafe ICollection<MemoryRegion> EnumerateMemoryRegions(nint hProcess, nint baseAddress) {
            var sizeOfStruct = sizeof(MEMORY_BASIC_INFORMATION64);
            var memoryInfoStructs = stackalloc MEMORY_BASIC_INFORMATION64[1];

            var mappedMemoryRegions = new List<MemoryRegion>();
            var bytesReturnedQuery = 0;

            while (true) {
                bytesReturnedQuery = VirtualQuery(hProcess, baseAddress, memoryInfoStructs, (uint)sizeOfStruct);
                if (bytesReturnedQuery == 0) {
                    break;
                }

                baseAddress += (nint)memoryInfoStructs->RegionSize;

                // We only want commited memory when scanning.
                if (memoryInfoStructs->State != PageState.MEM_COMMIT) {
                    continue;
                }

                ConvertFlags(memoryInfoStructs, out var readFlag, out var writeFlag, out var executeFlag);
                mappedMemoryRegions.Add(new MemoryRegion {
                    StartAddress = memoryInfoStructs->BaseAddress,
                    EndAddress = memoryInfoStructs->BaseAddress + memoryInfoStructs->RegionSize,
                    MemorySize = memoryInfoStructs->RegionSize,
                    ReadFlag = readFlag,
                    WriteFlag = writeFlag,
                    ExecuteFlag = executeFlag
                });
            }

            return mappedMemoryRegions;
        }

        private static unsafe void ConvertFlags(MEMORY_BASIC_INFORMATION64* memoryInfoStructs, out bool readFlag, out bool writeFlag, out bool executeFlag) {
            readFlag = memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READ) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_READONLY) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_READWRITE);

            writeFlag = memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_READWRITE) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_WRITECOPY) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_WRITECOPY);

            executeFlag = memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READ) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                        memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_WRITECOPY);
        }
    }
}
