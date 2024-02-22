namespace PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces {
    public interface IGlobalObjectsTracker {
        event EventHandler<ObjectDestroyedEventArgs>? OnObjectDestroyed;

        nint[] GetLoadedObjects();
        nint[] GetMarkedObjects();
        bool IsObjectDestroyed(nint uObjectAddress);
        bool IsObjectMarkedForCollection(nint uObjectAddress);
        void SynchroniseObjectPool();
    }
}