using System.Diagnostics;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

using static PalworldManagedModFramework.Sdk.Services.Memory.Windows.NativeFunctions;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Windows {
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

        private unsafe ICollection<MemoryRegion> EnumerateMemoryRegions(nint hProcess, nint baseAddress) {
            var sizeOfStruct = sizeof(MEMORY_BASIC_INFORMATION64);
            var memoryInfoStructs = (MEMORY_BASIC_INFORMATION64*)Marshal.AllocHGlobal(sizeOfStruct);

            try {
                var mappedMemoryRegions = new List<MemoryRegion>();
                var bytesReturnedQuery = 0;

                while (true) {
                    bytesReturnedQuery = NativeFunctions.VirtualQuery(hProcess, baseAddress, memoryInfoStructs, (uint)sizeOfStruct);

                    if (bytesReturnedQuery == 0) {
                        break;
                    }


                    var readFlag = memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READ) ||
                                    memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                                    memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_READONLY) ||
                                    memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_READWRITE);

                    var writeFlag = memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_READWRITE) ||
                                    memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                                    memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_WRITECOPY) ||
                                    memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_WRITECOPY);

                    var executeFlag = memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE) ||
                                      memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READ) ||
                                      memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                                      memoryInfoStructs->Protect.HasFlag(MemoryProtection.PAGE_EXECUTE_WRITECOPY);

                    baseAddress += (nint)memoryInfoStructs->RegionSize;

                    // We only want commited memory when scanning.
                    if (memoryInfoStructs->State != PageState.MEM_COMMIT) {
                        continue;
                    }

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
            finally {
                Marshal.FreeHGlobal((nint)memoryInfoStructs);
            }
        }
    }
}
