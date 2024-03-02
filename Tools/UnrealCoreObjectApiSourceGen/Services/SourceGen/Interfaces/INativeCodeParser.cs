using UnrealCoreObjectApiSourceGen.Models;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen {
    public interface INativeCodeParser {
        bool ParseSourceFile(string file, out LibClangTranslationRecord? libClangTranslationRecord);
    }
}