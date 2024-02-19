using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Services.Detour.Models {

    public record InstalledHook : IDisposable {
        public InstalledHook(byte[] originalCodes, nint pHook, int hookSize, nint redirect, nint pTrampoline) {
            OriginalCodes = originalCodes;
            PHook = pHook;
            HookSize = hookSize;
            Redirect = redirect;
            PTrampoline = pTrampoline;
        }

        public byte[] OriginalCodes { get; }
        public nint PHook { get; }
        public int HookSize { get; }
        public nint Redirect { get; }
        public nint PTrampoline { get; }

        public void Dispose() {
            //TODO: Setup calling uninstall on parent factory.
            Marshal.FreeHGlobal((nint)PTrampoline);
        }
    }
}
