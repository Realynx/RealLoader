using Polly.Registry;

namespace UnrealCoreObjectApiSourceGen.Models {
    public class NativeSyntaxTree {
        public List<HeaderFileTree> HeaderFiles { get; } = new();
    }

    public class HeaderFileTree {
        public string HeaderFileName { get; set; }
        public List<StructTree> Structs { get; } = new();
        public List<ClassTree> Classes { get; } = new();
    }

    public class StructTree {
        public AccessTree PublicMembers { get; init; }
        public AccessTree PrivateMembers { get; init; }
    }

    public class ClassTree {
        public List<ClassTree> Parents { get; } = new();
        public AccessTree PublicMembers { get; init; }
        public AccessTree PrivateMembers { get; init; }
    }

    public class AccessTree {
        public List<FieldTree> Fields { get; } = new();
    }

    public class FieldTree {
        public string MemberName { get; init; }
        public string MemberType { get; init; }
    }

    public class PreProcessorTree {
        public enum DirectiveType { If, Endif, Define, Undef, Else, Elif, Include }
        public DirectiveType Type { get; init; }
        public string Condition { get; init; }
    }

}
