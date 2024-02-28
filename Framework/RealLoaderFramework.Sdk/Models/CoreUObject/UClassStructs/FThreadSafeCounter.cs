using System.Runtime.InteropServices;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FThreadSafeCounter {
        public int Counter;
    }
}
