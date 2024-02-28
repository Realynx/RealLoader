using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Interfaces {
    public interface IStackWalker {
        ICollection<CustomStackFrame> WalkStackFrames(int frames = 10);
    }
}