using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public abstract class UObjectInterop {
        private readonly IUnrealReflection _unrealReflection;
        private readonly IGlobalObjectsTracker _globalObjectsTracker;

        // TODO: Expose UObject creation to the user via a factory service
        public UObjectInterop(nint address, IUnrealReflection unrealReflection, IGlobalObjectsTracker globalObjectsTracker) {
            Address = address;
            _unrealReflection = unrealReflection;
            _globalObjectsTracker = globalObjectsTracker;
        }

        public bool Disposed {
            get {
                return _globalObjectsTracker.IsObjectDestroyed(_addressUnsafe);
            }
        }

        private nint _addressUnsafe;

        public nint Address {
            get {
                if (Disposed) {
                    throw new ObjectDisposedException($"{_addressUnsafe}", $"The object at 0x{_addressUnsafe:X} does not exist.");
                }

                return _addressUnsafe;
            }
            private set {
                _addressUnsafe = value;
            }
        }

        public nint RegisterInUnreal() {
            // TODO: Address = Service.RegisterInUnreal(this);
            return _addressUnsafe;
        }

        public unsafe void ProcessEvent(nint functionStruct, void* arguments) {
            // TODO: Service.ProcessEvent(Address, functionStruct, arguments);
        }

        public unsafe nint GetFunctionStructPointer(Index functionIndex) {
            return (nint)_unrealReflection.GetFunctionAtIndex((UClass*)Address, functionIndex);
        }

        public nint GetObjectAddress<T>(ref T obj) {
            if (obj is UObjectInterop uObjectInterop) {
                return uObjectInterop.Address;
            }

            if (obj is nint pointer) {
                return pointer;
            }

            if (typeof(T).IsValueType) {
                // TODO: This may not be possible depending on how the objects are consumed by UE
                // return Service.GetOrCreatePooledUObject(obj);
                throw new NotImplementedException("Casting C# value types to Unreal objects is not yet implemented.");
            }

            throw new Exception($"An object not of type {nameof(UObjectInterop)} was passed. Type: {typeof(T).FullName ?? typeof(T).Name}.");
        }

        public void Dispose() {
            if (!Disposed) {
                // TODO: Service.DeleteInUnreal(this);
            }
        }
    }
}