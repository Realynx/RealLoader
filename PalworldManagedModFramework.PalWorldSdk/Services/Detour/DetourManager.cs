using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public class DetourManager : IDetourManager {
        private readonly ILogger _logger;
        private readonly IInstructionPatcher _instructionPatcher;
        private readonly IStackDetourService _stackDetourService;

        private readonly HashSet<DetourRecord> _installedDetours = new();
        private readonly Dictionary<ManagedDetourInfo, DetourRecord> _managedDetours = new();

        public DetourManager(ILogger logger, IInstructionPatcher instructionPatcher, IStackDetourService stackDetourService) {
            _logger = logger;
            _instructionPatcher = instructionPatcher;
            _stackDetourService = stackDetourService;
        }

        public DetourRecord PrepareDetour(ManagedDetourInfo managedDetourInfo, nint pFunction) {

            var pManagedFunction = Marshal.GetFunctionPointerForDelegate(managedDetourInfo.GetDetourDelegate);

            DetourRecord? detourRecord;
            switch (managedDetourInfo.DetourType) {
                case DetourType.Stack:
                    detourRecord = _stackDetourService.PrepareDetour(pFunction, pManagedFunction);
                    break;

                case DetourType.Jmp_IP:
                    // TODO:
                    detourRecord = null;
                    break;

                case DetourType.VTable:
                    // TODO:
                    detourRecord = null;
                    break;

                default:
                    detourRecord = null;
                    break;
            }

            if (detourRecord is null) {
                throw new Exception($"Failed to prepare hooked function! '{managedDetourInfo.DetourMethod.Name}'");
            }

            _logger.Debug($"[0x{detourRecord.PFunction:X}]Prepared Hook: {managedDetourInfo.DetourMethod.Name}, " +
                $"[0x{detourRecord.PTrampoline:X}]Trampoline: {managedDetourInfo.TrampolineDelegate.Name}");

            _managedDetours.Add(managedDetourInfo, detourRecord);
            managedDetourInfo.TrampolineDelegate.SetValue(null, detourRecord.PTrampoline);

            return detourRecord;
        }

        public bool InstallDetour(DetourRecord detourRecord) {
            if (!_installedDetours.Add(detourRecord)) {
                _logger.Error($"Tried to add duplicate detour at: 0x{detourRecord.PFunction:X}, Skipping duplicate detour.");
                return false;
            }

            var overWrittenCodes = _instructionPatcher.PatchInstructions(detourRecord.PFunction, detourRecord.DetourCodes);
            var correctPatch = AreEqual(detourRecord.OriginalCodes, overWrittenCodes);
            return correctPatch;
        }

        public bool UninstallDetour(DetourRecord detourRecord) {
            if (!_installedDetours.Remove(detourRecord)) {
                _logger.Error("Tried uninstalling a detour that does not exist! Skipping unknown detour.");
                return false;
            }

            var overWrittenCodes = _instructionPatcher.PatchInstructions(detourRecord.PFunction, detourRecord.OriginalCodes);
            var correctPatch = AreEqual(detourRecord.DetourCodes, overWrittenCodes);
            return correctPatch;
        }

        private static bool AreEqual(byte[] codes, byte[] overWrittenInstructions) {
            if (overWrittenInstructions.Length <= codes.Length) {
                return overWrittenInstructions.AsSpan(0, overWrittenInstructions.Length).SequenceEqual(codes);
            }

            return false;
        }
    }
}
