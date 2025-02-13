using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Interfaces;
using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Models;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenInterfaceNodeFactory : ICodeGenInterfaceNodeFactory {
        private readonly ILogger _logger;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;

        public CodeGenInterfaceNodeFactory(ILogger logger, ICodeGenAttributeNodeFactory attributeNodeFactory) {
            _logger = logger;
            _attributeNodeFactory = attributeNodeFactory;
        }

        public CodeGenInterfaceNode GenerateICreatableUObject(string className) {
            var name = nameof(ICreatableUObject<UObjectInterop>);

            var genericTypes = new[] {
                className
            };

            var methodAttributes = new[] {
                _attributeNodeFactory.GenerateAttribute(COMPILER_GENERATED_ATTRIBUTE)
            };

            var methodModifiers = "static";

            var methodName = $"{name}<{string.Join(", ", genericTypes)}>.{nameof(ICreatableUObject<UObjectInterop>.Create)}";

            var methodArgs = new[] {
                new CodeGenArgumentNode {
                    type = $"{nameof(UObject)}*",
                    name = CONSTRUCTOR_ADDRESS_NAME,
                },
                new CodeGenArgumentNode {
                    type = nameof(IUnrealReflection),
                    name = CONSTRUCTOR_UNREAL_REFLECTION_NAME,
                },
                new CodeGenArgumentNode {
                    type = nameof(IGlobalObjectsTracker),
                    name = CONSTRUCTOR_GLOBAL_OBJECTS_TRACKER_NAME,
                }
            };

            var methodBody = new[] {
                $"return new {className}((nint){CONSTRUCTOR_ADDRESS_NAME}, {CONSTRUCTOR_UNREAL_REFLECTION_NAME}, {CONSTRUCTOR_GLOBAL_OBJECTS_TRACKER_NAME});"
            };

            var bodyTypes = new[] {
                $"{nameof(UObject)}*",
                nameof(IUnrealReflection),
                nameof(IGlobalObjectsTracker),
            };

            var methodNode = new[] {
                new CodeGenMethodNode {
                    attributes = methodAttributes,
                    modifier = methodModifiers,
                    returnType = className,
                    name = methodName,
                    arguments = methodArgs,
                    body = methodBody,
                    bodyTypes = bodyTypes,
                }
            };

            return new CodeGenInterfaceNode {
                name = name,
                genericTypes = genericTypes,
                methodNodes = methodNode,
            };
        }
    }
}