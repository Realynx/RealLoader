using UnrealCoreObjectApiSourceGen.Models;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen {
    public interface ILanguageConverter {
        void ConvertLanugague(string buildDirectory, LibClangTranslationRecord libClangTranslationRecord, ConversionLanguage conversionLanguage);
    }
}