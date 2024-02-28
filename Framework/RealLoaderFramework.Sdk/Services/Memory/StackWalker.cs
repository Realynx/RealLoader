using System.Runtime.CompilerServices;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory {
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
            var memoryRegions = _memoryMapper.FindMemoryRegions()
                .Where(i => i.ExecuteFlag)
                .ToArray();

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

                // rsp is int*, this is increment 4 bytes
                rsp++;
            }

            return stackFrameList;
        }
    }
}
