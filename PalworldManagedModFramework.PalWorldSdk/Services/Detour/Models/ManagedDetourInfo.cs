using System.Linq.Expressions;
using System.Reflection;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Services.Memory;

namespace PalworldManagedModFramework.Sdk.Services.Detour.Models {
    public record ManagedDetourInfo {
        public ManagedDetourInfo(MethodInfo detourMethod, FieldInfo trampolineDelegate, DetourType detourType) {
            DetourMethod = detourMethod;
            TrampolineDelegate = trampolineDelegate;
            DetourType = detourType;
        }

        public MethodInfo DetourMethod { get; }
        public FieldInfo TrampolineDelegate { get; }
        public DetourType DetourType { get; }

        public Delegate GetDetourDelegate {
            get {
                return DetourMethod.CreateDelegate(Expression.GetDelegateType(
                    (from parameter in DetourMethod.GetParameters() select parameter.ParameterType)
                    .Concat([DetourMethod.ReturnType])
                    .ToArray()));
            }
        }

        public DetourAttribute DetourAttribute {
            get {
                return DetourMethod.GetCustomAttribute<DetourAttribute>()!;
            }
        }
    }
}
