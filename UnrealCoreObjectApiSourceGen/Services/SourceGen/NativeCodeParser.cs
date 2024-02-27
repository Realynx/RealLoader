using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;

using ClangSharp.Interop;

using Serilog;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen {
    public unsafe class NativeCodeParser {
        private readonly ILogger _logger;

        public NativeCodeParser(ILogger logger) {
            _logger = logger;
        }

        public unsafe bool ParseSourceFile(string file) {
            var index = clang.createIndex(0, 0);

            var argumentString = Marshal.StringToCoTaskMemUTF8("C:\\Users\\poofi\\source\\repos\\PalworldManagedModFramework\\UnrealCoreObjectApiSourceGen\\LocalHeaderFiles\\UObject\\Class.h");
            CXTranslationUnitImpl* translationUnit = clang.parseTranslationUnit(index, (sbyte*)argumentString, null, 0, null, 0, (uint)CXTranslationUnit_Flags.CXTranslationUnit_None);
            Marshal.FreeCoTaskMem(argumentString);

            if (translationUnit is null) {
                _logger.Error("Could not parse the file!");
                return false;
            }

            var cursor = clang.getTranslationUnitCursor(translationUnit);

            var nodeVisitFuncPtr = (delegate* unmanaged[Cdecl]<CXCursor, CXCursor, void*, CXChildVisitResult>)&NodeVisit;
            clang.visitChildren(cursor, nodeVisitFuncPtr, null);

            clang.disposeTranslationUnit(translationUnit);
            clang.disposeIndex(index);
            return true;
        }


        // Ensure the method signature matches the expected unmanaged function pointer signature
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static CXChildVisitResult NodeVisit(CXCursor cursor, CXCursor parent, void* clientData) {
            // Correctly retrieve the spelling and kind of the cursor
            var spelling = clang.getCursorSpelling(cursor).ToString();
            var displayName = clang.getCursorDisplayName(cursor).ToString();

            var kind = clang.getCursorKindSpelling(clang.getCursorKind(cursor)).ToString();
            if (kind is "StructDecl" or "FieldDecl") {
                Console.WriteLine($"Node: {spelling}: Node Type: {kind}");
            }



            return CXChildVisitResult.CXChildVisit_Recurse;
        }
    }
}
