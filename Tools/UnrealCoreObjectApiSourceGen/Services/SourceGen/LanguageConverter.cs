using UnrealCoreObjectApiSourceGen.Models;
using UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen {
    public enum ConversionLanguage {
        CSharp,
        Lua
    }

    public class LanguageConverter : ILanguageConverter {
        private readonly ICSharpConverter _cSharpConverter;

        public LanguageConverter(ICSharpConverter cSharpConverter) {
            _cSharpConverter = cSharpConverter;
        }

        public void ConvertLanugague(string buildDirectory, LibClangTranslationRecord libClangTranslationRecord, ConversionLanguage conversionLanguage) {
            if (!Directory.Exists(buildDirectory)) {
                Directory.CreateDirectory(buildDirectory);
            }

            switch (conversionLanguage) {
                case ConversionLanguage.CSharp:
                    _cSharpConverter.Convert(libClangTranslationRecord, buildDirectory);
                    break;

                case ConversionLanguage.Lua:

                    break;

                default:
                    Console.WriteLine("Unsupported convertion language!");
                    break;
            }
        }
    }
}
