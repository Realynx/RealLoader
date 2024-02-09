using System.Runtime.InteropServices;
using System.Text;

namespace PalworldManagedModFramework.UnrealSdk.Services.Structs {
    public class GNamesStructs {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FName {
            public FNameEntryId comparisonIndex;
            public int number;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FNameEntryId {
            public ushort lowerOrderValue;
            public ushort higherOrderValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FNameEntry {
            public FNameEntryHeader header;
            public byte StringPointer;
        }

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
                    return (byte)((headerValue >> 1) & 0x1F);
                }
            }

            public ushort Len {
                get {
                    return (ushort)(headerValue >> 6);
                }
            }
        }
    }
}
