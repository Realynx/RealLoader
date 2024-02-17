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
        internal const string CLASS = "class";
        internal const string PUBLIC = "public";
        internal const string PRIVATE = "private";
        internal const string PROTECTED = "protected";
        internal const string INTERNAL = "internal";
        internal const string STATIC = "static";
        internal const string UNSAFE = "unsafe";
        internal const string VOID = "void";
        internal const string INT_PTR = "nint";
        internal const string GET = "get";
        internal const string SET = "set";
        internal const string DELEGATE = "delegate";
        internal const string CODE_NAMESPACE = "DotNetSdkBuilderMod.Generated";
        internal const string ADDRESS_FIELD_NAME = "Address";
        internal const string FULLY_QUALIFIED_TYPE_PATH_ATTRIBUTE = "FullTypePath";
        internal const string COMPATIBLE_GAME_VERSION_ATTRIBUTE = "CompatibleGameVersion";
        internal const string DEPRECATED_ATTRIBUTE = "Obsolete";
        internal const char INDENT = ' ';
        internal const int INDENT_SIZE = 4;
        internal const string CORE_U_OBJECT_EXTRA_MEMBERS = """
// Framework members

public Object(nint address) {
    Address = address;
}

public bool Disposed { get; private set; } = true;

private nint _addressUnsafe;
public nint Address {
    get {
        if (Disposed) {
            throw new ObjectDisposedException();
        }

        return _addressUnsafe;
    }
    private set {
        _addressUnsafe = value;

        if (_addressUnsafe == IntPtr.Zero) {
            Disposed = true;
        }
        else {
            Disposed = false;
        }
    }
}

public nint RegisterInUnreal() {
    Address = Service.RegisterInUnreal(this);
    return _addressUnsafe;
}

public void OnObjectRemovedFromGlobalObjectPool(object sender, ObjectRemovedEventArgs e) {
    if (e.address == _addressUnsafe) {
        Address = IntPtr.Zero;
    }
}

public void ProcessEvent(nint function, void* arguments) {
    // TODO
}

private bool _disposing;

public void Dispose() {
    if (!disposing) {
        _disposing = true;
        Service.DeleteInUnreal(this);
    }
}
"""; // TODO: Compile as a regular class and use it to replace Script/CoreUObject/Object
    }
}