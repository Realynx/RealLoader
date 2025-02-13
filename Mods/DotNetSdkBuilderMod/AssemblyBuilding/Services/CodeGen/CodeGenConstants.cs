using RealLoaderFramework.Sdk.Models;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    internal static class CodeGenConstants {
        //internal const string NEW_LINE = "\n"; // or Environment.NewLine for OS-specific new lines
        //internal const string TAB = "\t"; // or use "    " for spaces, depending on your coding standards
        internal const string VOID = "void";
        internal const string PROJECT = "Project";
        internal const string PROPERTY_GROUP = "PropertyGroup";
        internal const string ITEM_GROUP = "ItemGroup";
        internal const string TARGET = "Target";
        internal const string COPY_ITEMS = "_CopyItems";
        internal const string COPY = "Copy";
        internal const string BUILD_OUTPUT_ENVIRONMENT_VARIABLE = "BuildDestinationDirectory";
        internal const string TARGET_FRAMEWORK = "TargetFramework";
        internal const string TARGET_FRAMEWORK_VALUE = "net9.0";
        internal const string ALLOW_UNSAFE_BLOCKS = "AllowUnsafeBlocks";
        internal const string IMPLICIT_USINGS = "ImplicitUsings";
        internal const string ADDRESS_OF_MANAGED_TYPE_WARNING_CODE = "CS8500";
        internal const string CODE_SOLUTION_NAME = "GeneratedSdk";
        internal const string CODE_NAMESPACE = "GeneratedSdk";
        internal const string CODE_GEN_INTEROP_NAMESPACE = "CodeGenInterop";
        internal const string CODE_GEN_INTEROP_RETURN_VALUE_NAME = "__returnValue";
        internal const string CODE_GEN_INTEROP_INVOKE_METHOD_NAME = "Invoke";
        internal const string U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE = $"{CODE_GEN_INTEROP_NAMESPACE}.Extensions";
        internal const string U_OBJECT_INTEROP_EXTENSIONS_CLASS_NAME = "UObjectInteropExtensions";
        internal const string EXECUTING_ADDRESS_FIELD_NAME = nameof(UObjectInterop.ExecutingAddress);
        internal const string ADDRESS_FIELD_NAME = nameof(UObjectInterop.Address);
        internal const string CONSTRUCTOR_ADDRESS_NAME = "address";
        internal const string CONSTRUCTOR_UNREAL_REFLECTION_NAME = "unrealReflection";
        internal const string CONSTRUCTOR_GLOBAL_OBJECTS_TRACKER_NAME = "globalObjectsTracker";
        internal const string OPERATOR_THIS_CLASS_NAME = "self"; // I don't like this but I can't think of anything better
        internal const string FULLY_QUALIFIED_TYPE_PATH_ATTRIBUTE = "FullTypePath";
        internal const string ORIGINAL_TYPE_NAME_ATTRIBUTE = "OriginalTypeName";
        internal const string ORIGINAL_MEMBER_NAME_ATTRIBUTE = "OriginalMemberName";
        internal const string COMPATIBLE_GAME_VERSION_ATTRIBUTE = "CompatibleGameVersion";
        internal const string COMPILER_GENERATED_ATTRIBUTE = "CompilerGenerated";
        internal const string DEPRECATED_ATTRIBUTE = "Obsolete";
        internal const char TAB = '\t';
        internal const char INDENT = ' ';
        internal const int INDENT_SIZE = 4;
    }
}