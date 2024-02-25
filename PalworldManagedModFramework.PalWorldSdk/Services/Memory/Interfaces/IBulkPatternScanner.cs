using System.Reflection;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IBulkPatternScanner {
        bool AddPattern(MemberInfo memberInfo, ByteCodePattern byteCodePattern, object instance = null);
        MemberInfo[] GetAllRegistredMembers();
        nint? GetMatchedAddress(ByteCodePattern byteCodePattern);
        ByteCodePattern GetRegistredByteCode(MemberInfo memberInfo);
        object GetRegistredTypeInstance(MemberInfo memberInfo);
        IBulkPatternScanner ScanAll();
    }
}