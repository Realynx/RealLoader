namespace RealLoader.UnrealEngine.CoreUObject {
    public class UClassRef : UStructRef {
        private readonly nint _pointer;

        public UClassRef(nint pointer) : base(pointer) {
            _pointer = pointer;
        }


    }
}
