namespace RealLoaderFramework.Sdk.Services.Memory.Models {
    public struct MemoryRegion {
        public ulong StartAddress;
        public ulong EndAddress;
        public ulong MemorySize;
        public bool ReadFlag;
        public bool WriteFlag;
        public bool ExecuteFlag;
    }
}
