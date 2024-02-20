using System.Reflection;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IBulkTypePatternScanner {
        bool AddPattern(MemberInfo memberInfo, ByteCodePattern byteCodePattern, object instance = null);
        MemberInfo[] GetAllRegistredMembers();
        nint? GetMatchedAddress(ByteCodePattern byteCodePattern);
        ByteCodePattern GetRegistredByteCode(MemberInfo memberInfo);
        object GetRegistredTypeInstance(MemberInfo memberInfo);
        IBulkTypePatternScanner ScanAll();
    }
}