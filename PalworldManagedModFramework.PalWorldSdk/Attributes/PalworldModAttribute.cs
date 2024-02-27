namespace PalworldManagedModFramework.Sdk.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class PalworldModAttribute : Attribute {
        public PalworldModAttribute(string modName, string author, string contactInfo, string semanticVersion, PalworldModType modType, string? repositoryUrl = null) {
            ModName = modName;
            Authors = [author];
            ContactInfo = contactInfo;
            SemanticVersion = semanticVersion;
            ModType = modType;
            RepositoryUrl = repositoryUrl;
        }

        public PalworldModAttribute(string modName, string[] authors, string contactInfo, string semanticVersion, PalworldModType modType, string? repositoryUrl = null) {
            ModName = modName;
            Authors = authors;
            ContactInfo = contactInfo;
            SemanticVersion = semanticVersion;
            ModType = modType;
            RepositoryUrl = repositoryUrl;
        }

        public string ModName { get; }
        public string[] Authors { get; }
        public string ContactInfo { get; }
        public string SemanticVersion { get; }
        public PalworldModType ModType { get; }
        public string? RepositoryUrl { get; }
    }

    [Flags]
    public enum PalworldModType {
        Client = 1,
        Server = 1 << 1,
        Universal = 1 << 2
    }
}
