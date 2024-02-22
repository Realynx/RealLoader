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

    public class LinuxDetourAttribute : DetourAttribute {
        public LinuxDetourAttribute(string pattern, DetourType detourType) : base(pattern, detourType) {
        }
    }

    public class WindowsDetourAttribute : DetourAttribute {
        public WindowsDetourAttribute(string pattern, DetourType detourType) : base(pattern, detourType) {
        }
    }
}
