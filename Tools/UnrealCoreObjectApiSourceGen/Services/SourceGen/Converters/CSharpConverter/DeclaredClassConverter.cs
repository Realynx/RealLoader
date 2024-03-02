using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using ClangSharp.Interop;

using UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter.Interfaces;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter {
    public unsafe class DeclaredClassConverter : IDeclaredClassConverter {
        private readonly IFieldConverter _fieldConverter;
        private static DeclaredClassConverter _instance;

        public DeclaredClassConverter(IFieldConverter fieldConverter) {
            _fieldConverter = fieldConverter;
            _instance = this;
        }

        public string ConvertClassToCSharp(CXCursor cursor, CXCursor parent) {
            var codeBuilder = new StringBuilder();

            var declareComponent = $"public unsafe struct {cursor.DisplayName} {{";
            codeBuilder.AppendLine(declareComponent);

            var nodeVisitFuncPtr = (delegate* unmanaged[Cdecl]<CXCursor, CXCursor, void*, CXChildVisitResult>)&NodeVisit;
            _ = clang.visitChildren(cursor, nodeVisitFuncPtr, &codeBuilder);

            codeBuilder.AppendLine("}");

            Console.WriteLine(codeBuilder);
            Console.WriteLine();

            return codeBuilder.ToString();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static CXChildVisitResult NodeVisit(CXCursor cursor, CXCursor parent, void* clientData) {
            if (cursor.Kind != CXCursorKind.CXCursor_FieldDecl) {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            var codeBulder = *(StringBuilder*)clientData;
            _instance._fieldConverter.ConvertFieldNode(cursor, parent, codeBulder);

            //var printingPolicy = clang.getCursorPrintingPolicy(cursor);
            //var prettyPrint = clang.getCursorPrettyPrinted(cursor, printingPolicy);
            //Console.WriteLine(prettyPrint);

            return CXChildVisitResult.CXChildVisit_Continue;
        }
    }
}
