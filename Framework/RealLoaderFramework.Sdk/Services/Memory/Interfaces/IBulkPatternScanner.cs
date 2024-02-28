using System.Reflection;

using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface IBulkPatternScanner {
        bool AddPattern(MemberInfo memberInfo, ByteCodePattern byteCodePattern, object instance = null);
        MemberInfo[] GetAllRegisteredMembers();
        nint? GetMatchedAddress(ByteCodePattern byteCodePattern);
        ByteCodePattern GetRegisteredByteCode(MemberInfo memberInfo);
        object GetRegisteredTypeInstance(MemberInfo memberInfo);
        IBulkPatternScanner ScanAll();
    }
}