using ClangSharp.Interop;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter.Interfaces {
    public interface IDeclaredClassConverter {
        string ConvertClassToCSharp(CXCursor cursor, CXCursor parent);
    }
}