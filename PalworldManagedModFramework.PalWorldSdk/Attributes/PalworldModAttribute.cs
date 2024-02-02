namespace PalworldManagedModFramework.PalWorldSdk.Attributes {
    public class PalworldModAttribute : Attribute {
        public PalworldModAttribute(string modName, string author, string discordAlias, string semanticVersion, bool serverMod) {
            ModName = modName;
            Author = author;
            DiscordAlias = discordAlias;
            SemanticVersion = semanticVersion;
            ServerMod = serverMod;
        }

        public string ModName { get; }
        public string Author { get; }
        public string DiscordAlias { get; }
        public string SemanticVersion { get; }
        public bool ServerMod { get; }
    }
}
