using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using ClangSharp.Interop;

using UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter.Interfaces;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter {
    public unsafe class DeclaredStructConverter : IDeclaredStructConverter {
        private static DeclaredStructConverter _instance;

        private readonly IFieldConverter _fieldConverter;

        public DeclaredStructConverter(IFieldConverter fieldConverter) {
            _fieldConverter = fieldConverter;
            _instance = this;
        }

        public string ConvertStructToCSharp(CXCursor cursor, CXCursor parent) {
            var codeBuilder = new StringBuilder();

            var declareComponent = GenerateDeclareComponent(cursor);
            codeBuilder.AppendLine(declareComponent);

            var nodeVisitFuncPtr = (delegate* unmanaged[Cdecl]<CXCursor, CXCursor, void*, CXChildVisitResult>)&NodeVisit;
            _ = clang.visitChildren(cursor, nodeVisitFuncPtr, &codeBuilder);

            codeBuilder.AppendLine("}");

            Console.WriteLine(codeBuilder);
            Console.WriteLine();

            return codeBuilder.ToString();
        }

        private static string GenerateDeclareComponent(CXCursor cursor) {
            var declareComponent = $"public unsafe struct {cursor.DisplayName} {{";
            return declareComponent;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static CXChildVisitResult NodeVisit(CXCursor cursor, CXCursor parent, void* clientData) {
            if (cursor.Kind != CXCursorKind.CXCursor_FieldDecl) {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            var codeBulder = *(StringBuilder*)clientData;
            _instance._fieldConverter.ConvertFieldNode(cursor, parent, codeBulder);

            return CXChildVisitResult.CXChildVisit_Continue;
        }
    }
}
