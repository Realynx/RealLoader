using System.Runtime.CompilerServices;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class StackWalker : IStackWalker {
        private readonly ILogger _logger;
        private readonly IMemoryMapper _memoryMapper;

        public StackWalker(ILogger logger, IMemoryMapper memoryMapper) {
            _logger = logger;
            _memoryMapper = memoryMapper;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public unsafe ICollection<CustomStackFrame> WalkStackFrames(int frames = 10) {
            int* stackPointer = stackalloc int[1];

            var rsp = stackPointer;
            var rbp = stackPointer + 2048;
            var memoryRegions = _memoryMapper.FindMemoryRegions().Where(i => i.ExecuteFlag);

            var stackFrameList = new List<CustomStackFrame>();
            while (rsp < rbp && stackFrameList.Count <= frames) {
                var callerAddress = *(nint*)rsp;
                var memoryRegion = memoryRegions.FirstOrDefault(i => callerAddress >= (nint)i.StartAddress && callerAddress <= (nint)i.EndAddress);

                if (memoryRegion.StartAddress != memoryRegion.EndAddress && memoryRegion.ExecuteFlag) {
                    stackFrameList.Add(new CustomStackFrame() {
                        StackAddress = (nint)rsp,
                        CallerAddress = callerAddress
                    });
                }

                rsp++;
            }

            return stackFrameList;
        }
    }
}
