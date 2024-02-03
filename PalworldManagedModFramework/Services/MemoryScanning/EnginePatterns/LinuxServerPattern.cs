namespace PalworldManagedModFramework.Services.MemoryScanning.EnginePatterns {
    public class LinuxServerPattern : IEnginePattern {
        public string NamePoolData { get; set; } = "53 89 FB 80 3D ? ? ? ? ? 75 ? BF |? ? ? ? E8 ? ? ? ? C6 05 ? ? ? ? 01";
        public string GUObjectArray { get; set; } = "0F 85 ? ? ? ? BF |? ? ? ? 4C 89 FE E8 ? ? ? ? E9";
    }
}
