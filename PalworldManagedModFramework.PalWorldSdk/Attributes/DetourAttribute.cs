using PalworldManagedModFramework.Sdk.Services.Memory;

namespace PalworldManagedModFramework.Sdk.Attributes {
    public class DetourAttribute : Attribute {
        public DetourAttribute(string pattern, DetourType detourType) {
            Pattern = pattern;
            DetourType = detourType;
        }

        public string Pattern { get; }
        public DetourType DetourType { get; }
    }
}
