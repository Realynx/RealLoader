using System.Runtime.InteropServices.Marshalling;

namespace RealLoaderFramework.Sdk.Services.Memory.Models;

[NativeMarshalling(typeof(HandleMarshaller))]
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