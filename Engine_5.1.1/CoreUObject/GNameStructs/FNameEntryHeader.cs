using System.Runtime.InteropServices;

namespace Engine_5._1._1.CoreUObject.GNameStructs {
    /// <summary>
    /// <see cref="https://docs.unrealengine.com/5.1/en-US/API/Runtime/Core/UObject/FNameEntryHeader/"/>
    /// |
    /// <see cref="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L198"/>
    /// </summary>
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
