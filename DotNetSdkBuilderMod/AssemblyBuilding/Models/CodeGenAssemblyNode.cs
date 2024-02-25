using System.Diagnostics;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    [DebuggerDisplay("{name}: {namespaces.Length} namespaces, {attributes?.Length ?? 0} attributes")]
    public class CodeGenAssemblyNode {
        public CodeGenNamespaceNode[] namespaces;
        public string name;
        public string[]? references;
        public CodeGenAttributeNode[]? attributes;
    }

    [DebuggerDisplay("{packageName}: {namespaces?.Length ?? 0} sub-namespaces, {classes?.Length ?? 0} classes, {imports?.Length ?? 0} imports")]
    public class CodeGenNamespaceNode {
        public CodeGenNamespaceNode[]? namespaces;
        public CodeGenClassNode[]? classes;

        public string packageName;
        public string fullName;
        public string name;
        public string[]? imports;
    }

    [DebuggerDisplay("{baseType} {name}: {constructorNodes?.Length ?? 0} constructors, {propertyNodes?.Length ?? 0} properties, {methodNodes?.Length ?? 0} methods, {operatorNodes?.Length ?? 0} operators")]
    public class CodeGenClassNode : CodeGenCodeObjectNode {
        public CodeGenConstructorNode[]? constructorNodes;
        public CodeGenPropertyNode[]? propertyNodes;
        public CodeGenMethodNode[]? methodNodes;
        public CodeGenOperatorNode[]? operatorNodes;

        public string? baseType;
    }

    [DebuggerDisplay("{name}: {arguments?.Length ?? 0} arguments")]
    public class CodeGenConstructorNode : CodeGenCodeObjectNode {
        public CodeGenArgumentNode[]? arguments;
        public string? baseConstructor;
        public string[]? body;
    }

    [DebuggerDisplay("{returnType} {name} {get}, {set}")]
    public class CodeGenPropertyNode : CodeGenCodeObjectNode {
        public string returnType;
        public string? get;
        public string? set;
    }

    [DebuggerDisplay("{returnType} {name}: {arguments?.Length ?? 0} arguments, {genericTypes?.Length ?? 0} generic types {bodyTypes?.Length ?? 0} body types")]
    public class CodeGenMethodNode : CodeGenCodeObjectNode {
        public string returnType;
        public string[]? genericTypes;
        public CodeGenArgumentNode[]? arguments;
        public string[]? body;
        public string[]? bodyTypes;
    }

    [DebuggerDisplay("{modifier} {returnType}({name}) => {result}")]
    public class CodeGenOperatorNode : CodeGenCodeObjectNode {
        public string returnType;
        public string result;
    }

    [DebuggerDisplay("[{name}({value})]")]
    public class CodeGenAttributeNode {
        public string name;
        public string? value;
    }

    [DebuggerDisplay("{modifier} {type} {name}, {attributes?.Length ?? 0} attributes")]
    public class CodeGenArgumentNode {
        public CodeGenAttributeNode[]? attributes;
        public string? modifier;
        public string type;
        public string name;
    }

    public class CodeGenCodeObjectNode {
        public CodeGenAttributeNode[]? attributes;
        public string modifier;
        public string name;
    }
}
