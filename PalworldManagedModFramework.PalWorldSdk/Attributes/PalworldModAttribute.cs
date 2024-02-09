namespace PalworldManagedModFramework.PalWorldSdk.Attributes {
    public class PalworldModAttribute : Attribute {
        public PalworldModAttribute(string modName, string author, string discordAlias, string semanticVersion, PalworldModType modType) {
            ModName = modName;
            Author = author;
            DiscordAlias = discordAlias;
            SemanticVersion = semanticVersion;
            ModType = modType;
        }

        public string ModName { get; }
        public string Author { get; }
        public string DiscordAlias { get; }
        public string SemanticVersion { get; }
        public PalworldModType ModType { get; }
    }

    [Flags]
    public enum PalworldModType {
        Client = 1,
        Server = 1 << 1,
        Universal = 1 << 2
    }
}
