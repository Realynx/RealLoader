using System.Reflection;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
    public unsafe class UObjectFactory {
        private static UObjectFactory? _singleInstance;

        private readonly ILogger _logger;
        private readonly IUnrealReflection _unrealReflection;
        private readonly IGlobalObjectsTracker _globalObjectsTracker;

        public UObjectFactory(ILogger logger, IUnrealReflection unrealReflection, IGlobalObjectsTracker globalObjectsTracker) {
            _logger = logger;
            _unrealReflection = unrealReflection;
            _globalObjectsTracker = globalObjectsTracker;

            _logger.Debug($"Setup single Instance {nameof(_singleInstance)}");
            _singleInstance = this;
        }

        public static T CreateProxy<T>(UObject* existingObject) where T : UObjectInterop {
            if (_singleInstance is null) {
                throw new NullReferenceException($"{nameof(_singleInstance)} was null");
            }

            const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance;
            var unrealReflection = _singleInstance._unrealReflection;
            var globalObjectsTracker = _singleInstance._globalObjectsTracker;
            return (T)Activator.CreateInstance(typeof(T), BINDING_FLAGS, null, [(nint)existingObject, unrealReflection, globalObjectsTracker], null)!;
        }
    }
}