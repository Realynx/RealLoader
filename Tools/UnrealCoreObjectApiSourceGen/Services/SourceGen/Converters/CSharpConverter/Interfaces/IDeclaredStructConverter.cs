using ClangSharp.Interop;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter.Interfaces {
    public interface IDeclaredStructConverter {
        string ConvertStructToCSharp(CXCursor cursor, CXCursor parent);
    }
}