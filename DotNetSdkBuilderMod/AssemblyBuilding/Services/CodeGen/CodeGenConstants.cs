namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    internal static class CodeGenConstants {
        internal const string OPEN_CURLY_BRACKET = "{";
        internal const string CLOSED_CURLY_BRACKET = "}";
        internal const string OPEN_ROUND_BRACKET = "(";
        internal const string CLOSED_ROUND_BRACKET = ")";
        internal const string OPEN_SQUARE_BRACKET = "[";
        internal const string CLOSED_SQUARE_BRACKET = "]";
        internal const string OPEN_ANGLE_BRACKET = "<";
        internal const string CLOSED_ANGLE_BRACKET = ">";
        internal const string WHITE_SPACE = " ";
        internal const string SEMICOLON = ";";
        internal const string COLON = ":";
        internal const string QUOTE = "\"";
        internal const string SINGLE_QUOTE = "'";
        internal const string COMMA = ",";
        internal const string DOT = ".";
        internal const string STAR = "*";
        internal const string AMPERSAND = "&";
        internal const string PLUS = "+";
        internal const string LAMBDA = "=>";
        internal const string EQUALS = "=";
        internal const string VALUE = "value";
        //internal const string NEW_LINE = "\n"; // or Environment.NewLine for OS-specific new lines
        //internal const string TAB = "\t"; // or use "    " for spaces, depending on your coding standards
        internal const string USING = "using";
        internal const string ASSEMBLY = "assembly";
        internal const string NAMESPACE = "namespace";
        internal const string OPERATOR = "operator";
        internal const string CLASS = "class";
        internal const string PUBLIC = "public";
        internal const string PRIVATE = "private";
        internal const string PROTECTED = "protected";
        internal const string INTERNAL = "internal";
        internal const string STATIC = "static";
        internal const string EXPLICIT = "explicit";
        internal const string IMPLICIT = "implicit";
        internal const string UNSAFE = "unsafe";
        internal const string ABSTRACT = "abstract";
        internal const string BASE = "base";
        internal const string VOID = "void";
        internal const string INT_PTR = "nint";
        internal const string DEFAULT = "default";
        internal const string INT = "int";
        internal const string GET = "get";
        internal const string SET = "set";
        internal const string NEW = "new";
        internal const string THIS = "this";
        internal const string RETURN = "return";
        internal const string DELEGATE = "delegate";
        internal const string CODE_NAMESPACE = "DotNetSdkBuilderMod.Generated";
        internal const string CODE_GEN_INTEROP_NAMESPACE = "CodeGenInterop";
        internal const string CODE_GEN_INTEROP_RETURN_VALUE_NAME = "__returnValue";
        internal const string CODE_GEN_INTEROP_INVOKE_METHOD_NAME = "Invoke";
        internal const string U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE = $"{CODE_GEN_INTEROP_NAMESPACE}.Extensions";
        internal const string U_OBJECT_INTEROP_EXTENSIONS_CLASS_NAME = "UObjectInteropExtensions";
        internal const string ADDRESS_FIELD_NAME = "Address";
        internal const string CONSTRUCTOR_ADDRESS_NAME = "address";
        internal const string CONSTRUCTOR_UNREAL_REFLECTION_NAME = "unrealReflection";
        internal const string CONSTRUCTOR_GLOBAL_OBJECTS_TRACKER_NAME = "globalObjectsTracker";
        internal const string OPERATOR_THIS_CLASS_NAME = "self"; // I don't like this but I can't think of anything better
        internal const string FULLY_QUALIFIED_TYPE_PATH_ATTRIBUTE = "FullTypePath";
        internal const string COMPATIBLE_GAME_VERSION_ATTRIBUTE = "CompatibleGameVersion";
        internal const string DEPRECATED_ATTRIBUTE = "Obsolete";
        internal const char INDENT = ' ';
        internal const int INDENT_SIZE = 4;
    }
}