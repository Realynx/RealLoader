using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Interfaces {
    public interface IStackWalker {
        ICollection<CustomStackFrame> WalkStackFrames(int frames = 10);
    }
}