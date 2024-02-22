namespace PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces {
    public interface IGlobalObjectsTracker {
        nint[] GetLoadedObjects();
        nint[] GetMarkedObjects();
        event EventHandler<ObjectDestroyedEventArgs>? OnObjectDestroyed;
        bool IsObjectDestroyed(nint uObjectAddress);
        bool IsObjectMarkedForCollection(nint uObjectAddress);
        void SynchroniseObjectPool();
    }
}