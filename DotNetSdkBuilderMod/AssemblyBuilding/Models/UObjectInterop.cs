using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public abstract class UObjectInterop {
        private readonly IUnrealReflection _unrealReflection;

        // TODO: Expose UObject creation to the user via a factory service
        public UObjectInterop(nint address, IUnrealReflection unrealReflection) {
            _unrealReflection = unrealReflection;
            Address = address;
        }

        public bool Disposed { get; private set; } = true;

        private nint _addressUnsafe;

        public nint Address {
            get {
                if (Disposed) {
                    throw new ObjectDisposedException($"{Address}", $"The object at {Address} does not exist.");
                }

                return _addressUnsafe;
            }
            private set {
                _addressUnsafe = value;

                if (_addressUnsafe == nint.Zero) {
                    Disposed = true;
                }
                else {
                    Disposed = false;
                }
            }
        }

        public nint RegisterInUnreal() {
            // TODO: Address = Service.RegisterInUnreal(this);
            return _addressUnsafe;
        }

        // TODO: public void OnObjectRemovedFromGlobalObjectPool(object sender, ObjectRemovedEventArgs e) {
        //     if (e.address == _addressUnsafe) {
        //         Address = IntPtr.Zero;
        //     }
        // }

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

        private bool _disposing;

        public void Dispose() {
            if (!_disposing) {
                _disposing = true;
                // TODO: Service.DeleteInUnreal(this);
            }
        }
    }
}