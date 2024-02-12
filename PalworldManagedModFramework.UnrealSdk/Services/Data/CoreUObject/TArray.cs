using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TArray<Type> {
        public nint** AllocatorInstance;
        public uint ArrayNum;
        public uint ArrayMax;

        public nint[] GetPtrArray {
            get {
                var ptrArray = new List<nint>();
                for (var x = 0; x < ArrayNum; x++) {

                    ptrArray.Add((*AllocatorInstance)[x]);
                }

                return ptrArray.ToArray();
            }
        }
    }
}
