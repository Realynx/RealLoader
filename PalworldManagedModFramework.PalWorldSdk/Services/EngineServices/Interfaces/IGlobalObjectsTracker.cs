namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
    public interface IGlobalObjectsTracker {
        nint[] GetLoadedObjects();
        nint[] GetMarkedObjects();
        bool IsObjectDestroyed(nint uObjectAddress);
        bool IsObjectMarkedForCollection(nint uObjectAddress);
    }
}