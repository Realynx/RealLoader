using Engine_5._1._1.CoreUObject.UClassStructs;

namespace RealLoader.UnrealEngine.CoreUObject {
    public class UClassRef : UStructRef {
        private readonly nint _pointer;

        public UClassRef(nint pointer) : base(pointer) {
            _pointer = pointer;
        }

        public void Foo<TUClass>(TUClass uClass) where TUClass : IUClass<IUStruct> {
            var uStruct = uClass.BaseUStruct;
        }
    }
}
