namespace RealLoaderFramework.Sdk.Services.Memory.Models;

internal readonly record struct Handle {
    public Handle(nuint pointer) {
        Pointer = pointer;
    }

    public nuint Pointer { get; }

    public bool IsNil {
        get {
            return Pointer == 0;
        }
    }
}