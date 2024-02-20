using System.Reflection;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IPatternResolver {
        nint? ResolvePattern(ByteCodePattern pattern);
        nint?[] ResolvePatterns(ByteCodePattern[] patterns);
    }
}