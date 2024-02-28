namespace RealLoaderFramework.Sdk.Services.Memory.Models {
    [Flags]
    public enum SimpleMemoryProtection {
        None = 0x0,
        Read = 1 << 1,
        Write = 1 << 2,
        Execute = 1 << 3
    }
}
