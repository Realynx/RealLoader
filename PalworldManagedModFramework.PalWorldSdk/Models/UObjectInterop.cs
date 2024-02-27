using System.Diagnostics.CodeAnalysis;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.EngineServices.UnrealHook;

namespace PalworldManagedModFramework.Sdk.Models {
    [RequiresUnreferencedCode("Used by source generated SDK")]
    public abstract unsafe class UObjectInterop {
        private readonly IUnrealReflection _unrealReflection;
        private readonly IGlobalObjectsTracker _globalObjectsTracker;

        /// <remarks>
        /// This constructor is not intended to be used by mod developers.
        /// </remarks>
        [RequiresUnreferencedCode("Used by source generated SDK")]
        protected internal UObjectInterop(nint address, IUnrealReflection unrealReflection, IGlobalObjectsTracker globalObjectsTracker) {
            Address = (UObject*)address;
            ExecutingAddress = (UStruct*)((UObject*)address)->baseObjectBaseUtility.baseUObjectBase.classPrivate;
            _unrealReflection = unrealReflection;
            _globalObjectsTracker = globalObjectsTracker;
        }

        public bool Disposed {
            [RequiresUnreferencedCode("Used by source generated SDK")]
            get {
                return _globalObjectsTracker.IsObjectDestroyed((nint)_addressUnsafe);
            }
        }

        protected internal UStruct* ExecutingAddress;

        private UObject* _addressUnsafe;

        public UObject* Address {
            [RequiresUnreferencedCode("Used by source generated SDK")]
            get {
                if (Disposed) {
                    throw new ObjectDisposedException($"{(nint)_addressUnsafe}", $"The object at 0x{(nint)_addressUnsafe:X} does not exist.");
                }

                return _addressUnsafe;
            }
            private set {
                _addressUnsafe = value;
            }
        }

        public bool TryRegisterInUnreal(out UObject* address) {
            if (Disposed) {
                address = null;
                return false;
            }

            // TODO: Address = Service.RegisterInUnreal(this);
            address = _addressUnsafe;
            return true;
        }

        [RequiresUnreferencedCode("Used by source generated SDK")]
        public void Invoke(UFunction* functionStruct, void* arguments) {
            UnrealHookManager.ProcessEvent(Address, functionStruct, arguments);
        }

        [RequiresUnreferencedCode("Used by source generated SDK")]
        public UFunction* GetFunctionStructPointer(Index functionIndex) {
            var result = _unrealReflection.GetFunctionAtIndex(ExecutingAddress, functionIndex);
            ExecutingAddress = (UStruct*)Address->baseObjectBaseUtility.baseUObjectBase.classPrivate;
            return result;
        }

        [RequiresUnreferencedCode("Used by source generated SDK")]
        public static nint GetObjectAddress<T>(ref T obj) {
            if (obj is UObjectInterop uObjectInterop) {
                return (nint)uObjectInterop.Address;
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