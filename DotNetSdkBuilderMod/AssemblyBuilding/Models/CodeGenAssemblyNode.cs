namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public class CodeGenAssemblyNode {
        public CodeGenNamespaceNode[] namespaces;
    }

    public class CodeGenNamespaceNode {
        public CodeGenNamespaceNode[] namespaces;
        public ClassNode[] classes;
    }

    public class CodeGenClassNode {
        public string attributes;

        public string modifer;
        public string name;

        public CodeGenPropertyNode[] propertyNodes;
        public CodeGenMethodNode[] methodNodes;
    }

    public class CodeGenPropertyNode {
        public string attributes;

        public string modifer;
        public string returnType;
        public string name;
        public string get;
        public string set;
    }

    public class CodeGenMethodNode {
        public string attributes;

        public string modifer;
        public string returnType;
        public string name;
        public string arguments;
    }
}
