using System.Numerics;

using Microsoft.Extensions.Logging;

using PalworldManagedModFramework.Services.Detour.AssemblerServices.Interfaces;
using PalworldManagedModFramework.Services.Detour.Interfaces;
using PalworldManagedModFramework.Services.Detour.Models;

namespace PalworldManagedModFramework.Services.Detour {
    public class StackHookService : IStackHookService {
        private readonly ILogger _logger;
        private readonly IShellCodeFactory _shellCodeFactory;
        private readonly IMemoryAllocate _memoryAllocate;
        private readonly List<InstalledHook> _installedHooks = new();

        public StackHookService(ILogger logger, IShellCodeFactory shellCodeFactory, IMemoryAllocate memoryAllocate) {
            _logger = logger;
            _shellCodeFactory = shellCodeFactory;
            _memoryAllocate = memoryAllocate;
        }

        public unsafe InstalledHook InstallHook(nint hookAddress, nint redirect) {
            var hookInstructionBytes = _shellCodeFactory.BuildStackDetour64(redirect);
            var instructionString = BitConverter.ToString(hookInstructionBytes).Replace("-", " ");
            _logger.LogDebug($"Hook Instructions: {instructionString}");

            var overwrittenCodes = OverwriteBytes(hookAddress, hookInstructionBytes);
            var reExecuteAddress = hookAddress + hookInstructionBytes.Length;

            var trampolineCodes = _shellCodeFactory.BuildTrampoline64(overwrittenCodes, reExecuteAddress);
            var trampoline = _memoryAllocate.Allocate(MemoryProtection.Execute, (uint)trampolineCodes.Length);

            var installedHook = new InstalledHook(overwrittenCodes, hookAddress, hookInstructionBytes.Length, redirect, trampoline);
            _installedHooks.Add(installedHook);
            return installedHook;
        }

        public unsafe void UninstallHook(InstalledHook installedHook) {
            _installedHooks.Remove(installedHook);
            // TODO: Check these bytes match the real detour bytes safe check?
            var detourBytes = OverwriteBytes(installedHook.PHook, installedHook.OriginalCodes);

            var originalDetour = _shellCodeFactory.BuildStackDetour64(installedHook.Redirect);
            if (!detourBytes.AsSpan().SequenceEqual(originalDetour)) {
                throw new Exception("The bytes overwritten did not contain the uninstalled hook.");
            }

            _memoryAllocate.Free(installedHook.PHook);
        }

        private unsafe byte[] OverwriteBytes(nint hookAddress, ReadOnlySpan<byte> hookInstructionBytes) {
            var length = hookInstructionBytes.Length;

            var destinationInstructions = new Span<byte>((byte*)hookAddress, length);
            var overwrittenInstructions = new byte[length];

            var i = 0;

            var vectorSize = Vector<byte>.Count;
            for (; i <= length - vectorSize; i += vectorSize) {
                var destinationOffset = destinationInstructions.Slice(i);
                var hookBytesOffset = hookInstructionBytes.Slice(i);

                new Vector<byte>(destinationOffset).CopyTo(overwrittenInstructions, i);
                new Vector<byte>(hookBytesOffset).CopyTo(destinationOffset);
            }

            for (; i < length; i++) {
                overwrittenInstructions[i] = destinationInstructions[i];
                destinationInstructions[i] = hookInstructionBytes[i];
            }

            return overwrittenInstructions;
        }
    }
}
