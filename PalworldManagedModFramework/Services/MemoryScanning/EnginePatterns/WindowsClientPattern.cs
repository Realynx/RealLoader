namespace PalworldManagedModFramework.Services.MemoryScanning.EnginePatterns {
    internal class WindowsClientPattern : IEnginePattern {
        public string NamePoolData { get; set; } = "48 8D 05 ? ? ? ? EB 13 48 8D 0D | ? ? ? ? E8 ? ? ? ? C6 05 ? ? ? ? ? 0F 10";

        public string GUObjectArray { get; set; } = "48 8B 05 | ? ? ? ? 48 8B 0C C8 4C 8D 04 D1 EB 03";
    }
}
