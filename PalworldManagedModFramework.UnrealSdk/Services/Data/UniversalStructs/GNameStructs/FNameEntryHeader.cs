using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs {
    [StructLayout(LayoutKind.Sequential, Size = 2)]
    public struct FNameEntryHeader {
        public ushort headerValue;

        public bool BIsWide {
            get {
                return (headerValue & 0x1) == 0x1;
            }
        }

        public byte LowercaseProbeHash {
            get {
                return (byte)(headerValue >> 1 & 0x1F);
            }
        }

        public ushort Len {
            get {
                return (ushort)(headerValue >> 6);
            }
        }
    }
}
