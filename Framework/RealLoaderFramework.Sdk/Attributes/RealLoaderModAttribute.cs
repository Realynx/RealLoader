namespace RealLoaderFramework.Sdk.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class RealLoaderModAttribute : Attribute {
        public RealLoaderModAttribute(string modName, string author, string contactInfo, string semanticVersion, RealLoaderModType modType, string? repositoryUrl = null) {
            ModName = modName;
            Authors = [author];
            ContactInfo = contactInfo;
            SemanticVersion = semanticVersion;
            ModType = modType;
            RepositoryUrl = repositoryUrl;
        }

        public RealLoaderModAttribute(string modName, string[] authors, string contactInfo, string semanticVersion, RealLoaderModType modType, string? repositoryUrl = null) {
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
        public RealLoaderModType ModType { get; }
        public string? RepositoryUrl { get; }
    }

    [Flags]
    public enum RealLoaderModType {
        Client = 1,
        Server = 1 << 1,
        Universal = 1 << 2
    }
}
