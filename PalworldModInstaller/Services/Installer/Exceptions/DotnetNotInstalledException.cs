namespace PalworldModInstaller.Services.Installer.Exceptions {
    [Serializable]
    internal class DotnetNotInstalledException : Exception {

        public DotnetNotInstalledException(string message) : base(message) {
        }
    }
}