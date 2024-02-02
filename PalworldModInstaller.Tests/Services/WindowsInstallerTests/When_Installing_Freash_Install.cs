using Xunit;

namespace PalworldModInstaller.Tests.Services.WindowsInstallerTests {
    public class When_Installing_Freash_Install : Using_WindowsInstaller_Service {
        protected override void Setup() {
            Directory.CreateDirectory(_palWin64Folder);
            File.WriteAllText(_palGamePE, string.Empty);
        }

        protected override void Act() {
            TestableImplementation.InstallFiles(_installerOptions);
        }

        [Fact]
        public void Was_Mods_Folder_Created() {
            try {
                var modsFolder = Path.Combine(_installerOptions.InstallLocation!, "ClrMods");
                Assert.True(Path.Exists(modsFolder));
            }
            finally {
                Dispose();
            }
        }

        [Fact]
        public void Was_Executable_Renamed() {
            try {
                var renamedExecutable = Path.Combine(_palWin64Folder, "Game-Palworld-Win64-Shipping.exe");
                Assert.True(Path.Exists(renamedExecutable));
            }
            finally {
                Dispose();
            }
        }

        [Fact]
        public void Was_Framework_Install_Dir_Created() {
            try {
                var managedModFrameworkFolder = Path.Combine(_palWin64Folder, "ManagedModFramework");
                Assert.True(Path.Exists(managedModFrameworkFolder));

            }
            finally {
                Dispose();
            }
        }
    }
}
