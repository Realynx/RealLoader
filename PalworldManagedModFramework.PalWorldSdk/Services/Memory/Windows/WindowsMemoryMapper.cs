using System.Diagnostics;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

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

        private unsafe ICollection<MemoryRegion> EnumerateMemoryRegions(nint hProcess, nint baseAddress) {
            var sizeOfStruct = sizeof(WindowsStructs.MEMORY_BASIC_INFORMATION64);
            var memoryInfoStructs = (WindowsStructs.MEMORY_BASIC_INFORMATION64*)Marshal.AllocHGlobal(sizeOfStruct);

            try {
                var mappedMemoryRegions = new List<MemoryRegion>();
                var bytesReturnedQuery = 0;

                while (true) {
                    bytesReturnedQuery = WindowsNativeMethods.VirtualQuery(hProcess, baseAddress, memoryInfoStructs, (uint)sizeOfStruct);
                    if (bytesReturnedQuery == 0) {
                        break;
                    }

                    var readFlag = memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_EXECUTE_READ) ||
                                    memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                                    memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_READONLY) ||
                                    memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_READWRITE);

                    var writeFlag = memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_READWRITE) ||
                                    memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                                    memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_WRITECOPY) ||
                                    memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_EXECUTE_WRITECOPY);

                    var executeFlag = memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_EXECUTE) ||
                                      memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_EXECUTE_READ) ||
                                      memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_EXECUTE_READWRITE) ||
                                      memoryInfoStructs->Protect.HasFlag(WindowsStructs.MemoryProtection.PAGE_EXECUTE_WRITECOPY);

                    mappedMemoryRegions.Add(new MemoryRegion {
                        StartAddress = memoryInfoStructs->BaseAddress,
                        EndAddress = memoryInfoStructs->BaseAddress + memoryInfoStructs->RegionSize,
                        MemorySize = memoryInfoStructs->RegionSize,
                        ReadFlag = readFlag,
                        WriteFlag = writeFlag,
                        ExecuteFlag = executeFlag
                    });

                    baseAddress = baseAddress + (nint)memoryInfoStructs->RegionSize;
                }

                return mappedMemoryRegions;
            }
            finally {
                Marshal.FreeHGlobal((nint)memoryInfoStructs);
            }
        }
    }
}
