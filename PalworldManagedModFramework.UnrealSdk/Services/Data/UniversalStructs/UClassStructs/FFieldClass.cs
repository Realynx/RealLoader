using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct FFieldClass {
        public FName name;
        public ulong id;
        public ulong castFlags;
        public EClassFlags classFlags;
        public FFieldClass* superClass;
        public FField* defaultObject;
        public FField* constructFn;
        public FThreadSafeCounter UniqueNameIndexCounter;
    }
}
