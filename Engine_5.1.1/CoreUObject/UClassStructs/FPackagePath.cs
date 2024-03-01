using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject.Flags;
using Engine_5._1._1.CoreUObject.GNameStructs;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FPackagePath {
        public FName packageName;
        public EPackageExtension headerExtension;
    }
}
