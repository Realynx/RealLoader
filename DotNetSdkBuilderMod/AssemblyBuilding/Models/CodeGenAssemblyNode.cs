using System.Diagnostics;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    [DebuggerDisplay("{name}: {namespaces.Length} namespaces, {attributes?.Length ?? 0} attributes")]
    public class CodeGenAssemblyNode {
        public CodeGenNamespaceNode[] namespaces;
        public string name;
        public string[]? attributes;
    }

    [DebuggerDisplay("{packageName}: {namespaces?.Length ?? 0} sub-namespaces, {classes?.Length ?? 0} classes, {imports?.Length ?? 0} imports")]
    public class CodeGenNamespaceNode {
        public CodeGenNamespaceNode[]? namespaces;
        public CodeGenClassNode[]? classes;

        public string packageName;
        public string name;
        public string[]? imports;
    }

    [DebuggerDisplay("{baseType} {name}: {propertyNodes?.Length ?? 0} props, {methodNodes?.Length ?? 0} methods, {operators?.Length ?? 0} operators")]
    public class CodeGenClassNode : CodeGenCodeObjectNode {
        public CodeGenPropertyNode[]? propertyNodes;
        public CodeGenMethodNode[]? methodNodes;
        public CodeGenOperatorNode[]? operators;

        public string? baseType;
    }

    [DebuggerDisplay("{returnType} {name} {get}, {set}")]
    public class CodeGenPropertyNode : CodeGenCodeObjectNode {
        public string returnType;
        public string? get;
        public string? set;
    }

    [DebuggerDisplay("{returnType} {name}: {arguments?.Length ?? 0} arguments")]
    public class CodeGenMethodNode : CodeGenCodeObjectNode {
        public string returnType;
        public (string type, string name)[]? arguments;
    }

    [DebuggerDisplay("{modifier} {returnType}({name}) => {result}")]
    public class CodeGenOperatorNode : CodeGenCodeObjectNode {
        public string returnType;
        public string result;
    }

    public class CodeGenCodeObjectNode {
        public string[]? attributes;
        public string modifier;
        public string name;
    }
}
