﻿using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Services.Detour.Models {

    public unsafe record InstalledHook : IDisposable {
        public InstalledHook(byte[] originalCodes, byte* pHook, int hookSize, nint redirect, byte* pTrampoline) {
            OriginalCodes = originalCodes;
            PHook = pHook;
            HookSize = hookSize;
            Redirect = redirect;
            PTrampoline = pTrampoline;
        }

        public byte[] OriginalCodes { get; }
        public byte* PHook { get; }
        public int HookSize { get; }
        public nint Redirect { get; }
        public byte* PTrampoline { get; }

        public void Dispose() {
            //TODO: Setup calling uninstall on parent factory.
            Marshal.FreeHGlobal((nint)PTrampoline);
        }
    }
}
