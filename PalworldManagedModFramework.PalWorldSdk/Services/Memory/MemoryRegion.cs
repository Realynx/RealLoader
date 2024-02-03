namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    public struct MemoryRegion {
        public ulong StartAddress;
        public ulong EndAddress;
        public ulong MemorySize;
        public bool ReadFlag;
        public bool WriteFlag;
        public bool ExecuteFlag;
    }
}
