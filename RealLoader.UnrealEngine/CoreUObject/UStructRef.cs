namespace RealLoader.UnrealEngine.CoreUObject {
    public class UStructRef : UFieldRef {
        private readonly nint _pointer;

        public UStructRef(nint pointer) : base(pointer) {
            _pointer = pointer;
        }
    }
}
