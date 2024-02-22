using PalworldManagedModFramework.Sdk.Services.Detour;

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
