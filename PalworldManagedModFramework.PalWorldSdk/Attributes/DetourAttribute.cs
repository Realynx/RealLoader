using PalworldManagedModFramework.Sdk.Services.Detour.Models;

namespace PalworldManagedModFramework.Sdk.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class DetourAttribute : Attribute {
        public DetourAttribute(DetourType detourType) {
            DetourType = detourType;
        }

        public DetourType DetourType { get; }
    }

    public class PatternDetourAttribute : DetourAttribute {
        public PatternDetourAttribute(string pattern, DetourType detourType) : base(detourType) {
            Pattern = pattern;
        }

        public string Pattern { get; }
    }

    public class EngineDetourAttribute : DetourAttribute {
        public EngineDetourAttribute(EngineFunction engineFunction, DetourType detourType) : base(detourType) {
            EngineFunction = engineFunction;
        }

        public EngineFunction EngineFunction { get; }
    }

    public class LinuxDetourAttribute : PatternDetourAttribute {
        public LinuxDetourAttribute(string pattern, DetourType detourType) : base(pattern, detourType) {
        }
    }

    public class WindowsDetourAttribute : PatternDetourAttribute {
        public WindowsDetourAttribute(string pattern, DetourType detourType) : base(pattern, detourType) {
        }
    }
}
