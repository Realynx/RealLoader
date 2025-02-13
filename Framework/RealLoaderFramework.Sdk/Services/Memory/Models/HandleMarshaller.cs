using System.Runtime.InteropServices.Marshalling;

namespace RealLoaderFramework.Sdk.Services.Memory.Models;

[CustomMarshaller(typeof(Handle), MarshalMode.Default, typeof(HandleMarshaller))]
internal static class HandleMarshaller {
    public static nuint ConvertToUnmanaged(Handle managed) {
        return managed.Pointer;
    }

    public static Handle ConvertToManaged(nuint unmanaged) {
        return new Handle(unmanaged);
    }
}