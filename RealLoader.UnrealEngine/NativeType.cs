namespace RealLoader.UnrealEngine {
    public unsafe class NativeType<TStruct> where TStruct : struct {
        private readonly TStruct* _pointer;

        public NativeType(nint pointer) {
            _pointer = (TStruct*)pointer;
        }

        protected TStruct* GetPointer() {
            return _pointer;
        }
    }
}
