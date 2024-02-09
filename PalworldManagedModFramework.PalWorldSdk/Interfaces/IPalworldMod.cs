using PalworldManagedModFramework.PalWorldSdk.Logging;

namespace PalworldManagedModFramework.PalWorldSdk.Interfaces {
    public interface IPalworldMod {
        /// <summary>
        /// This function is called when your assembly/mod has been loaded into the system.
        /// </summary>
        public void Load();

        /// <summary>
        /// Graceful request for unloading. If this function duration is longer than 5 seconds, the mod will be terminated forcefully.
        /// </summary>
        public void Unload();
    }
}
