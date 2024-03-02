using UnrealCoreObjectApiSourceGen.Models;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter {
    public interface ICSharpConverter {
        string Convert(LibClangTranslationRecord libClangTranslationRecord, string directory);
    }
}