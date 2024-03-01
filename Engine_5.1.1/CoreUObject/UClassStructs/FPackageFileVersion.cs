using System.Runtime.InteropServices;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FPackageFileVersion {
        public int fileVersionUE4;
        public int fileVersionUE5;
    }
}
