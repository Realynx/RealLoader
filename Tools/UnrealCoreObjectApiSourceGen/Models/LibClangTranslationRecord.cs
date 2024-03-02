using ClangSharp.Interop;

namespace UnrealCoreObjectApiSourceGen.Models {
    public unsafe record LibClangTranslationRecord {
        public LibClangTranslationRecord(CXCursor rootCursor, CXTranslationUnitImpl* translationUnit, void* index) {
            RootCursor = rootCursor;
            TranslationUnit = (nint)translationUnit;
            Index = (nint)index;
        }

        public CXCursor RootCursor { get; }
        public nint TranslationUnit { get; }
        public nint Index { get; }

        public void Dispose() {
            clang.disposeTranslationUnit((CXTranslationUnitImpl*)TranslationUnit);
            clang.disposeIndex((void*)Index);
        }
    }
}
