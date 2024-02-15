namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public class CodeGenAssemblyNode {
        public CodeGenNamespaceNode[] namespaces;
    }

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
