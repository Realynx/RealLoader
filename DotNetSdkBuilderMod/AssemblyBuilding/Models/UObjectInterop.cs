namespace DotNetSdkBuilderMod.AssemblyBuilding.Models {
    public abstract class UObjectInterop {
        public UObjectInterop(nint address) {
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

                if (_addressUnsafe == IntPtr.Zero) {
                    Disposed = true;
                }
                else {
                    Disposed = false;
                }
            }
        }

        public nint RegisterInUnreal() {
            // Address = Service.RegisterInUnreal(this);
            return _addressUnsafe;
        }

        // public void OnObjectRemovedFromGlobalObjectPool(object sender, ObjectRemovedEventArgs e) {
        //     if (e.address == _addressUnsafe) {
        //         Address = IntPtr.Zero;
        //     }
        // }

        public unsafe void ProcessEvent(int functionAddress, void* arguments) {
            // Service.ProcessEvent(Address, functionAddress, arguments);
        }

        private bool _disposing;

        public void Dispose() {
            if (!_disposing) {
                _disposing = true;
                // Service.DeleteInUnreal(this);
            }
        }
    }
}