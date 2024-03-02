using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using ClangSharp.Interop;

using UnrealCoreObjectApiSourceGen.Models;
using UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter.Interfaces;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter {
    public unsafe class CSharpConverter : ICSharpConverter {

        private static CSharpConverter? _singleInstance;
        private readonly IDeclaredClassConverter _declaredClassConverter;
        private readonly IDeclaredStructConverter _declaredStructConverter;

        public CSharpConverter(IDeclaredClassConverter declaredClassConverter, IDeclaredStructConverter declaredStructConverter) {
            _declaredClassConverter = declaredClassConverter;
            _declaredStructConverter = declaredStructConverter;
            _singleInstance = this;
        }

        public string Convert(LibClangTranslationRecord libClangTranslationRecord, string directory) {
            var codeBuilder = new StringBuilder();

            var nodeVisitFuncPtr = (delegate* unmanaged[Cdecl]<CXCursor, CXCursor, void*, CXChildVisitResult>)&NodeVisit;
            _ = clang.visitChildren(libClangTranslationRecord.RootCursor, nodeVisitFuncPtr, &codeBuilder);

            return "";
        }


        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private unsafe static CXChildVisitResult NodeVisit(CXCursor cursor, CXCursor parent, void* clientData) {
            var codeBulder = *(StringBuilder*)clientData;

            static void FindRecursiveParents(CXCursor cursor, List<CXCursor> bases) {
                for (var x = 0u; x < cursor.NumBases; x++) {
                    var classBase = cursor.GetBase(x);

                    bases.Add(classBase);

                    FindRecursiveParents(classBase, bases);
                }
            }

            if (cursor.Kind == CXCursorKind.CXCursor_ClassDecl) {
                var classBases = new List<CXCursor>();
                FindRecursiveParents(cursor, classBases);
                _singleInstance!._declaredClassConverter.ConvertClassToCSharp(cursor, parent);
                //if (classBases.Any(i => i.DisplayName.CString == "UObjectBase")) {
                //    return CXChildVisitResult.CXChildVisit_Continue;
                //}
            }

            if (cursor.Kind == CXCursorKind.CXCursor_StructDecl) {
                _singleInstance!._declaredStructConverter.ConvertStructToCSharp(cursor, parent);
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            // var displayName = clang.getCursorDisplayName(cursor).ToString();
            //var location = clang.getCursorLocation(cursor);
            //location.GetFileLocation(out var file, out var line, out var column, out var _);
            // var spelling = clang.getCursorSpelling(cursor).ToString();
            //Console.WriteLine($"Node: {spelling}: Node Type: {cursor.Kind}, Display Name: {displayName}");
            //Console.WriteLine($"{Path.GetFileName(file.Name.CString)}::{spelling} - ({line},{column}), {cursor.Kind}");

            //Console.WriteLine($"({cursor.NominatedBaseClass}){cursor.DisplayName}");
            // Console.WriteLine($"    children: {cursor.NumChildren}");


            //var printingPolicy = clang.getCursorPrintingPolicy(cursor);
            //var prettyPrint = clang.getCursorPrettyPrinted(cursor, printingPolicy);
            //Console.WriteLine(prettyPrint);


            // Console.WriteLine(cursor.DisplayName);
            return CXChildVisitResult.CXChildVisit_Continue;
        }
    }
}
