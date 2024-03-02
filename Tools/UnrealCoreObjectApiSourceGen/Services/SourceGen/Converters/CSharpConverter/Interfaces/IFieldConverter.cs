using System.Text;

using ClangSharp.Interop;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter {
    public interface IFieldConverter {
        void ConvertFieldNode(CXCursor cursor, CXCursor parent, StringBuilder codeBuilder);
    }
}