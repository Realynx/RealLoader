using System.Linq.Expressions;
using System.Reflection;

using RealLoaderFramework.Sdk.Attributes;

namespace RealLoaderFramework.Sdk.Services.Detour.Models {
    public record ManagedDetourInfo {
        public ManagedDetourInfo(MethodInfo detourMethod, FieldInfo trampolineDelegate, DetourType detourType) {
            DetourMethod = detourMethod;
            TrampolineDelegate = trampolineDelegate;
            DetourType = detourType;

            GetDetourDelegate = DetourMethod.CreateDelegate(Expression.GetDelegateType(
                (from parameter in DetourMethod.GetParameters() select parameter.ParameterType)
                .Append(DetourMethod.ReturnType)
                .ToArray()));
        }

        public MethodInfo DetourMethod { get; }
        public FieldInfo TrampolineDelegate { get; }
        public DetourType DetourType { get; }

        public Delegate GetDetourDelegate { get; }

        public DetourAttribute DetourAttribute {
            get {
                return DetourMethod.GetCustomAttribute<DetourAttribute>()!;
            }
        }
    }
}
