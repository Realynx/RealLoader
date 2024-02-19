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

        public unsafe void InstallHook(nint hookAddress, nint redirect) {
            var hookInstructionBytes = _shellCodeFactory.BuildStackDetour64(redirect);
            var instructionString = BitConverter.ToString(hookInstructionBytes).Replace("-", " ");
            _logger.LogDebug($"Hook Instructions: {instructionString}");

            var overwrittenCodes = OverwriteBytes(hookAddress, hookInstructionBytes);
            var reExecuteAddress = hookAddress + hookInstructionBytes.Length;

            var trampolineCodes = _shellCodeFactory.BuildTrampoline64(overwrittenCodes, reExecuteAddress);
            var trampoline = _memoryAllocate.Allocate(MemoryProtection.Execute, (uint)trampolineCodes.Length);

            var installedHook = new InstalledHook(overwrittenCodes, hookAddress, hookInstructionBytes.Length, redirect, trampoline);
            _installedHooks.Add(installedHook);
        }

        public unsafe void UninstallHook(InstalledHook installedHook) {
            _installedHooks.Remove(installedHook);
            // TODO: Check these bytes match the real detour bytes safe check?
            var detourBytes = OverwriteBytes((nint)installedHook.PHook, installedHook.OriginalCodes);

            _memoryAllocate.Free((nint)installedHook.PHook);
        }

        private unsafe byte[] OverwriteBytes(nint hookAddress, byte[] hookInstructionBytes) {
            byte* patternInstructions = (byte*)hookAddress;
            var overwrittenInstructions = new byte[hookInstructionBytes.Length];
            for (var x = 0; x < overwrittenInstructions.Length; x++) {
                overwrittenInstructions[x] = patternInstructions[x];
                patternInstructions[x] = hookInstructionBytes[x];
            }

            return overwrittenInstructions;
        }
    }
}
