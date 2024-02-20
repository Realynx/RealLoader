using PalworldManagedModFramework.Sdk.Services.Memory;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Models {

    public record DetourRecord {
        public DetourRecord(byte[] detourCodes, byte[] originalCodes, nint pHook, nint redirect, nint pTrampoline, DetourType detourType) {
            DetourCodes = detourCodes;
            OriginalCodes = originalCodes;
            PFunction = pHook;
            DetourAddress = redirect;
            PTrampoline = pTrampoline;
            DetourType = detourType;
        }

        public byte[] DetourCodes { get; }
        public byte[] OriginalCodes { get; }

        public nint PFunction { get; }
        public nint DetourAddress { get; }
        public nint PTrampoline { get; }

        public DetourType DetourType { get; }
    }
}
