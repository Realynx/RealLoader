using System.Diagnostics;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    [DebuggerDisplay("{name}: {namespaces?.Length ?? 0} namespaces")]
    public class CodeGenAssemblyNode {
        public CodeGenNamespaceNode[] namespaces;
        public string name;
    }

    [DebuggerDisplay("{fullNameSpace}: {namespaces?.Length ?? 0} sub-namespaces, {classes?.Length ?? 0} classes")]
    public class CodeGenNamespaceNode {
        public CodeGenNamespaceNode[] namespaces;
        public CodeGenClassNode[] classes;

        public string fullNameSpace;
        public string name;
        public string imports;
    }

    [DebuggerDisplay("{baseType} {name}: {propertyNodes?.Length ?? 0} props, {methodNodes?.Length ?? 0} methods")]
    public class CodeGenClassNode : CodeGenCodeObjectNode {
        public string baseType;
        public CodeGenPropertyNode[] propertyNodes;
        public CodeGenMethodNode[] methodNodes;
    }

    [DebuggerDisplay("{returnType} {name} {get}, {set}")]
    public class CodeGenPropertyNode : CodeGenCodeObjectNode {
        public string returnType;
        public string get;
        public string set;
    }

    [DebuggerDisplay("{returnType} {name}: {arguments?.Length ?? 0} arguments")]
    public class CodeGenMethodNode : CodeGenCodeObjectNode {
        public string returnType;
        public (string type, string name)[] arguments;
    }

    public class CodeGenCodeObjectNode {
        public string attributes;
        public string modifer;
        public string name;
    }
}
