using System.Diagnostics;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public class CodeGenAssemblyNode {
        public CodeGenNamespaceNode[] namespaces;
    }

    [DebuggerDisplay("{nameSpace}: {namespaces?.Length ?? 0} sub-namespaces, {classes?.Length ?? 0} classes")]
    public class CodeGenNamespaceNode {
        public CodeGenNamespaceNode[] namespaces;
        public CodeGenClassNode[] classes;

        public string nameSpace;
        public string imports;
    }

    public class CodeGenClassNode : CodeGenCodeObjectNode {
        public string baseType;
        public CodeGenPropertyNode[] propertyNodes;
        public CodeGenMethodNode[] methodNodes;
    }

    public class CodeGenPropertyNode : CodeGenCodeObjectNode {
        public string returnType;
        public string get;
        public string set;
    }

    public class CodeGenMethodNode : CodeGenCodeObjectNode {
        public string returnType;
        public string arguments;
    }

    public class CodeGenCodeObjectNode {
        public string attributes;
        public string modifer;
        public string name;
    }
}
