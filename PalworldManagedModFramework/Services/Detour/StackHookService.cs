using System.Runtime.InteropServices;

using Microsoft.Extensions.Logging;

using PalworldManagedModFramework.Services.Detour.AssemblerServices.Interfaces;
using PalworldManagedModFramework.Services.Detour.Models;

namespace PalworldManagedModFramework.Services.Detour {
    public class StackHookService {
        private readonly ILogger _logger;
        private readonly IShellCodeFactory _shellCodeFactory;
        private readonly List<InstalledHook> _installedHooks = new();

        public StackHookService(ILogger logger, IShellCodeFactory shellCodeFactory) {
            _logger = logger;
            _shellCodeFactory = shellCodeFactory;
        }

        public unsafe void InstallHook(nint hookAddress, nint redirect) {
            var hookInstructionBytes = _shellCodeFactory.BuildStackDetour64(redirect);
            var instructionString = BitConverter.ToString(hookInstructionBytes).Replace("-", " ");
            _logger.LogDebug($"Hook Instructions: {instructionString}");

            var overwrittenCodes = OverwriteBytes(hookAddress, hookInstructionBytes);
            var reExecuteAddress = hookAddress + hookInstructionBytes.Length;

            var reEntryInstructions = _shellCodeFactory.BuildTrampoline64(overwrittenCodes, reExecuteAddress);
            var trampoline = Marshal.AllocHGlobal(reEntryInstructions.Length);

            //TODO: Set Vprotect To execute

            var installedHook = new InstalledHook(overwrittenCodes, (byte*)hookAddress, hookInstructionBytes.Length, redirect, (byte*)trampoline);
            _installedHooks.Add(installedHook);
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

        public unsafe void UninstallHook(InstalledHook installedHook) {
            _installedHooks.Remove(installedHook);
            var detourBytes = OverwriteBytes((nint)installedHook.PHook, installedHook.OriginalCodes);

            //TODO: Reset Vprotect?

            Marshal.FreeHGlobal((nint)installedHook.PTrampoline);
        }
    }
}
