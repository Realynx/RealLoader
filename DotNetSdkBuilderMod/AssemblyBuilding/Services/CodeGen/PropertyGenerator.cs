using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class PropertyGenerator : IPropertyGenerator {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;

        public PropertyGenerator(ILogger logger, IGlobalObjects globalObjects) {
            _logger = logger;
            _globalObjects = globalObjects;
        }

        public unsafe void GenerateProperty(StringBuilder codeBuilder, FProperty property) {
            var modifiers = GetPropertyModifiers(property);

            codeBuilder.AppendIndented(modifiers, 2);
            codeBuilder.Append(WHITE_SPACE);

            var className = _globalObjects.GetNameString(property.baseFField.classPrivate->ObjectName);
            codeBuilder.Append(className);
            codeBuilder.Append(WHITE_SPACE);

            var propertyName = _globalObjects.GetNameString(property.ObjectName);
            codeBuilder.Append(propertyName);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(OPEN_CURLY_BRACKET);

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(GETTER);
            codeBuilder.Append(SEMICOLON);

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(SETTER);
            codeBuilder.Append(SEMICOLON);

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(CLOSED_CURLY_BRACKET);
        }

        private string GetPropertyModifiers(FProperty property) {
            // TODO: At some point we may want to get more details here.
            return PUBLIC;
        }
    }
}
